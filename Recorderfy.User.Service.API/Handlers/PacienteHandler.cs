// Recorderfy.User.Service.Api/Handlers/PacienteHandler.cs
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using System.Text.Json;

namespace Recorderfy.User.Service.API.Handlers;

public class PacienteHandler
{
    public async Task<object> HandleCreateAsync(
        IPacienteService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var dto = JsonSerializer.Deserialize<CreatePacienteDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.CreatePacienteAsync(dto!);

            logger.LogInformation(
                "[{CorrelationId}] Paciente creado exitosamente - ID: {Id}",
                correlationId, result.IdUsuario);

            return new
            {
                success = true,
                data = result,
                message = "Paciente creado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al crear paciente",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<object> HandleUpdateAsync(
        IPacienteService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);
            var dto = JsonSerializer.Deserialize<CreatePacienteDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.UpdatePacienteAsync(id, dto!);

            logger.LogInformation(
                "[{CorrelationId}] Paciente actualizado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                data = result,
                message = "Paciente actualizado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al actualizar paciente",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<object> HandleDeleteAsync(
        IPacienteService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.DeletePacienteAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Paciente eliminado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                message = "Paciente eliminado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al eliminar paciente",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<object> HandleGetByIdAsync(
        IPacienteService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.GetPacienteByIdAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Paciente obtenido - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                data = result,
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al obtener paciente",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<object> HandleGetAllAsync(
        IPacienteService service,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var result = await service.GetAllPacientesAsync();

            logger.LogInformation(
                "[{CorrelationId}] Pacientes obtenidos - Total: {Count}",
                correlationId, result.Count());

            return new
            {
                success = true,
                data = result,
                count = result.Count(),
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al obtener pacientes",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }
}