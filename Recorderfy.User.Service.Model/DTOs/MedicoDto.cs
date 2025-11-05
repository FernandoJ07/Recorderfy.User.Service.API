using System;

namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// DTO para respuesta de Médico (sin ciclos de referencia)
/// </summary>
public class MedicoDto
{
    public Guid IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string NroDocumento { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Genero { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public string? Estado { get; set; }
    public string? FotoPerfil { get; set; }
    
    // Datos de tipo de documento y rol (sin navegaciones completas)
    public TipoDocumentoDto? TipoDocumento { get; set; }
    public RolDto? Rol { get; set; }
    
    // Campos específicos de Médico
    public string? Especialidad { get; set; }
    public string? CentroMedico { get; set; }
    public bool? NotificacionesActivadas { get; set; }
    public string? FirmaDigital { get; set; }
}
