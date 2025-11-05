namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// DTO para crear un médico (hereda campos de Usuario)
/// </summary>
public class CreateMedicoDto : BaseUsuarioDto
{
    // Campos específicos de Médico
    public string? Especialidad { get; set; }
    public string? CentroMedico { get; set; }
    public bool NotificacionesActivadas { get; set; } = true;
}
