// Recorderfy.User.Service.Api/Handlers/MedicoHandler.cs
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using System.Text.Json;

namespace Recorderfy.User.Service.API.Handlers;

public class MedicoHandler
{
    public async Task<object> HandleCreateAsync(
        IMedicoService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var dto = JsonSerializer.Deserialize<CreateMedicoDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.CreateMedicoAsync(dto!);

            logger.LogInformation(
                "[{CorrelationId}] Médico creado exitosamente - ID: {Id}",
                correlationId, result.IdUsuario);

            return new
            {
                success = true,
                data = result,
                message = "Médico creado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al crear médico",
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
        IMedicoService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);
            var dto = JsonSerializer.Deserialize<CreateMedicoDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.UpdateMedicoAsync(id, dto!);

            logger.LogInformation(
                "[{CorrelationId}] Médico actualizado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                data = result,
                message = "Médico actualizado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al actualizar médico",
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
        IMedicoService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.DeleteMedicoAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Médico eliminado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                message = "Médico eliminado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al eliminar médico",
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
        IMedicoService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.GetMedicoByIdAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Médico obtenido - ID: {Id}",
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
                "[{CorrelationId}] Error al obtener médico",
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
        IMedicoService service,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var result = await service.GetAllMedicosAsync();

            logger.LogInformation(
                "[{CorrelationId}] Médicos obtenidos - Total: {Count}",
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
                "[{CorrelationId}] Error al obtener médicos",
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