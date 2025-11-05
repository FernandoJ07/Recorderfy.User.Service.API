namespace Recorderfy.User.Service.Model.Entities;

public partial class Usuario
{
    public Guid IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public int IdTipoDocumento { get; set; }
    public string NroDocumento { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string PasswordHash { get; set; } = null!;
    public int IdRol { get; set; }
    public string? Genero { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public string? Estado { get; set; }
    public string? FotoPerfil { get; set; }

    // Navegaciones a entidades relacionadas
    public virtual Rol IdRolNavigation { get; set; } = null!;
    public virtual TipoDocumento IdTipoDocumentoNavigation { get; set; } = null!;
}