using System;

namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// DTO para respuesta de Paciente (sin ciclos de referencia)
/// </summary>
public class PacienteDto
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
    
    // Campos específicos de Paciente
    public string? DiagnosticoInicial { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public string? ObservacionesClinicas { get; set; }
    public string? FotoReferencia { get; set; }
    
    // Referencias simples (solo IDs y nombres, no objetos completos)
    public Guid? IdCuidador { get; set; }
    public string? NombreCuidador { get; set; }
    public Guid? IdMedico { get; set; }
    public string? NombreMedico { get; set; }
}
