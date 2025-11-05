// Recorderfy.User.Service.Api/Consumer/RabbitMqConsumer.cs
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.API.Handlers;
using System.Text;
using System.Text.Json;

namespace Recorderfy.User.Service.Api.Consumer;

public class RabbitMqConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string ExchangeName = "user-exchange";

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IConfiguration config,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _config = config;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Consumer para User API iniciando...");

        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest"
        };

        try
        {
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var queueName = "user-api-queue";
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            // Routing keys para Paciente, Medico y Cuidador
            var routingKeys = new[]
            {
                // Paciente
                "user.api.paciente.create",
                "user.api.paciente.update",
                "user.api.paciente.delete",
                "user.api.paciente.getById",
                "user.api.paciente.getAll",
                
                // Medico
                "user.api.medico.create",
                "user.api.medico.update",
                "user.api.medico.delete",
                "user.api.medico.getById",
                "user.api.medico.getAll",
                
                // Cuidador
                "user.api.cuidador.create",
                "user.api.cuidador.update",
                "user.api.cuidador.delete",
                "user.api.cuidador.getById",
                "user.api.cuidador.getAll"
            };

            foreach (var routingKey in routingKeys)
            {
                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    arguments: null,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Binding creado: {RoutingKey}", routingKey);
            }

            await _channel.BasicQosAsync(0, 1, false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId ?? Guid.NewGuid().ToString();
                var replyTo = ea.BasicProperties.ReplyTo;

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation(
                        "[{CorrelationId}] Mensaje recibido con routing key: {RoutingKey}",
                        correlationId, routingKey);

                    var response = await ProcessMessageAsync(routingKey, message, correlationId, stoppingToken);

                    if (!string.IsNullOrEmpty(replyTo))
                    {
                        await SendResponseAsync(replyTo, correlationId, response, stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);

                    _logger.LogInformation(
                        "[{CorrelationId}] Mensaje procesado exitosamente",
                        correlationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[{CorrelationId}] Error procesando mensaje",
                        correlationId);

                    if (!string.IsNullOrEmpty(replyTo))
                    {
                        var errorResponse = new
                        {
                            success = false,
                            error = ex.Message,
                            timestamp = DateTime.UtcNow
                        };
                        await SendResponseAsync(replyTo, correlationId, errorResponse, stoppingToken);
                    }

                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("RabbitMQ Consumer esperando mensajes en cola: {QueueName}", queueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fatal en RabbitMQ Consumer");
            throw;
        }
    }

    private async Task<object> ProcessMessageAsync(
        string routingKey,
        string message,
        string correlationId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        // Determinar qué handler usar según el routing key
        var parts = routingKey.Split('.');
        if (parts.Length < 3) return CreateErrorResponse("Routing key inválido");

        var entity = parts[2]; // paciente, medico, cuidador
        var action = parts.Length > 3 ? parts[3] : "unknown";

        return entity switch
        {
            "paciente" => await ProcessPacienteAsync(action, message, correlationId, scope),
            "medico" => await ProcessMedicoAsync(action, message, correlationId, scope),
            "cuidador" => await ProcessCuidadorAsync(action, message, correlationId, scope),
            _ => CreateErrorResponse($"Entidad desconocida: {entity}")
        };
    }

    private async Task<object> ProcessPacienteAsync(
        string action,
        string message,
        string correlationId,
        IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<IPacienteService>();
        var handler = new PacienteHandler();

        return action switch
        {
            "create" => await handler.HandleCreateAsync(service, message, correlationId, _logger),
            "update" => await handler.HandleUpdateAsync(service, message, correlationId, _logger),
            "delete" => await handler.HandleDeleteAsync(service, message, correlationId, _logger),
            "getById" => await handler.HandleGetByIdAsync(service, message, correlationId, _logger),
            "getAll" => await handler.HandleGetAllAsync(service, correlationId, _logger),
            _ => CreateErrorResponse($"Acción desconocida: {action}")
        };
    }

    private async Task<object> ProcessMedicoAsync(
        string action,
        string message,
        string correlationId,
        IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<IMedicoService>();
        var handler = new MedicoHandler();

        return action switch
        {
            "create" => await handler.HandleCreateAsync(service, message, correlationId, _logger),
            "update" => await handler.HandleUpdateAsync(service, message, correlationId, _logger),
            "delete" => await handler.HandleDeleteAsync(service, message, correlationId, _logger),
            "getById" => await handler.HandleGetByIdAsync(service, message, correlationId, _logger),
            "getAll" => await handler.HandleGetAllAsync(service, correlationId, _logger),
            _ => CreateErrorResponse($"Acción desconocida: {action}")
        };
    }

    private async Task<object> ProcessCuidadorAsync(
        string action,
        string message,
        string correlationId,
        IServiceScope scope)
    {
        var service = scope.ServiceProvider.GetRequiredService<ICuidadorService>();
        var handler = new CuidadorHandler();

        return action switch
        {
            "create" => await handler.HandleCreateAsync(service, message, correlationId, _logger),
            "update" => await handler.HandleUpdateAsync(service, message, correlationId, _logger),
            "delete" => await handler.HandleDeleteAsync(service, message, correlationId, _logger),
            "getById" => await handler.HandleGetByIdAsync(service, message, correlationId, _logger),
            "getAll" => await handler.HandleGetAllAsync(service, correlationId, _logger),
            _ => CreateErrorResponse($"Acción desconocida: {action}")
        };
    }

    private object CreateErrorResponse(string errorMessage)
    {
        return new
        {
            success = false,
            error = errorMessage,
            timestamp = DateTime.UtcNow
        };
    }

    private async Task SendResponseAsync(
        string replyTo,
        string correlationId,
        object response,
        CancellationToken cancellationToken)
    {
        var responseBody = JsonSerializer.Serialize(response);
        var responseBytes = Encoding.UTF8.GetBytes(responseBody);

        var replyProps = new BasicProperties
        {
            CorrelationId = correlationId
        };

        await _channel!.BasicPublishAsync(
            exchange: "",
            routingKey: replyTo,
            mandatory: false,
            basicProperties: replyProps,
            body: responseBytes,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "[{CorrelationId}] Respuesta enviada a {ReplyTo}",
            correlationId, replyTo);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Consumer deteniéndose...");

        if (_channel != null)
            await _channel.CloseAsync(cancellationToken);

        if (_connection != null)
            await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}