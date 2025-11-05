// Recorderfy.User.Service.Api/Handlers/CuidadorHandler.cs
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using System.Text.Json;

namespace Recorderfy.User.Service.API.Handlers;

public class CuidadorHandler
{
    public async Task<object> HandleCreateAsync(
        ICuidadorService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var dto = JsonSerializer.Deserialize<CreateCuidadorDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.CreateCuidadorAsync(dto!);

            logger.LogInformation(
                "[{CorrelationId}] Cuidador creado exitosamente - ID: {Id}",
                correlationId, result.IdUsuario);

            return new
            {
                success = true,
                data = result,
                message = "Cuidador creado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al crear cuidador",
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
        ICuidadorService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);
            var dto = JsonSerializer.Deserialize<CreateCuidadorDto>(
                request.GetProperty("Data").GetRawText());

            var result = await service.UpdateCuidadorAsync(id, dto!);

            logger.LogInformation(
                "[{CorrelationId}] Cuidador actualizado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                data = result,
                message = "Cuidador actualizado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al actualizar cuidador",
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
        ICuidadorService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.DeleteCuidadorAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Cuidador eliminado - ID: {Id}",
                correlationId, id);

            return new
            {
                success = true,
                message = "Cuidador eliminado exitosamente",
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al eliminar cuidador",
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
        ICuidadorService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var id = Guid.Parse(request.GetProperty("Id").GetString()!);

            var result = await service.GetCuidadorByIdAsync(id);

            logger.LogInformation(
                "[{CorrelationId}] Cuidador obtenido - ID: {Id}",
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
                "[{CorrelationId}] Error al obtener cuidador",
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
        ICuidadorService service,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var result = await service.GetAllCuidadoresAsync();

            logger.LogInformation(
                "[{CorrelationId}] Cuidadores obtenidos - Total: {Count}",
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
                "[{CorrelationId}] Error al obtener cuidadores",
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