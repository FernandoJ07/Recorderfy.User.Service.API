// Recorderfy.User.Service.Api/Handlers/AuthHandler.cs
using Recorderfy.User.Service.BLL.Interfaces;
using Recorderfy.User.Service.Model.DTOs;
using System.Text.Json;

namespace Recorderfy.User.Service.API.Handlers;

public class AuthHandler
{
    public async Task<object> HandleLoginAsync(
        IUsuarioService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var dto = JsonSerializer.Deserialize<LoginDto>(
                request.GetProperty("Data").GetRawText());

            if (dto == null)
            {
                logger.LogWarning(
                    "[{CorrelationId}] LoginDto no pudo ser deserializado",
                    correlationId);

                return new
                {
                    success = false,
                    error = "Datos de login inválidos",
                    timestamp = DateTime.UtcNow
                };
            }

            var result = await service.LoginAsync(dto);

            if (result.Success)
            {
                logger.LogInformation(
                    "[{CorrelationId}] Login exitoso - Email: {Email}",
                    correlationId, dto.Email);
            }
            else
            {
                logger.LogWarning(
                    "[{CorrelationId}] Login fallido - Email: {Email}",
                    correlationId, dto.Email);
            }

            return new
            {
                success = result.Success,
                data = result.Usuario,
                message = result.Message,
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al procesar login",
                correlationId);

            return new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<object> HandleGetByDocumentAndRoleAsync(
        IUsuarioService service,
        string message,
        string correlationId,
        ILogger logger)
    {
        try
        {
            var request = JsonSerializer.Deserialize<JsonElement>(message);
            var nroDocumento = request.GetProperty("NroDocumento").GetString();
            var idRol = request.GetProperty("IdRol").GetInt32();

            if (string.IsNullOrEmpty(nroDocumento))
            {
                logger.LogWarning(
                    "[{CorrelationId}] Número de documento no proporcionado",
                    correlationId);

                return new
                {
                    success = false,
                    error = "Número de documento requerido",
                    timestamp = DateTime.UtcNow
                };
            }

            var usuario = await service.GetByDocumentAndRoleAsync(nroDocumento, idRol);

            if (usuario == null)
            {
                logger.LogInformation(
                    "[{CorrelationId}] Usuario no encontrado - Doc: {NroDocumento}, Rol: {IdRol}",
                    correlationId, nroDocumento, idRol);

                return new
                {
                    success = false,
                    message = "Usuario no encontrado",
                    timestamp = DateTime.UtcNow
                };
            }

            logger.LogInformation(
                "[{CorrelationId}] Usuario encontrado - Doc: {NroDocumento}, Rol: {IdRol}",
                correlationId, nroDocumento, idRol);

            return new
            {
                success = true,
                data = usuario,
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "[{CorrelationId}] Error al buscar usuario por documento y rol",
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