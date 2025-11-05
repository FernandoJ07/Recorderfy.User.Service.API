namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// DTO para crear un cuidador (hereda campos de Usuario)
/// </summary>
public class CreateCuidadorDto : BaseUsuarioDto
{
    // Campos específicos de Cuidador
    public string? RelacionConPaciente { get; set; }
    public string? Direccion { get; set; }
    public bool NotificacionesActivadas { get; set; } = true;
}
