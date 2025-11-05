using System;

namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// DTO para crear un paciente (hereda campos de Usuario)
/// </summary>
public class CreatePacienteDto : BaseUsuarioDto
{
    // Campos específicos de Paciente
    public string? DiagnosticoInicial { get; set; }
    public Guid? IdCuidador { get; set; }
    public Guid? IdMedico { get; set; }
    public string? ObservacionesClinicas { get; set; }
    public string? FotoReferencia { get; set; }
}
