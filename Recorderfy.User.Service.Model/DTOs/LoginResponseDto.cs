namespace Recorderfy.User.Service.Model.DTOs
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public UsuarioDto? Usuario { get; set; }
    }

    public class UsuarioDto
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
        
        // Objetos completos de tipo de documento y rol
        public TipoDocumentoDto? TipoDocumento { get; set; }
        public RolDto? Rol { get; set; }
    }
}
