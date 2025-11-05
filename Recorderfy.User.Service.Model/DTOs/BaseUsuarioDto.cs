using System;

namespace Recorderfy.User.Service.Model.DTOs;

/// <summary>
/// Clase base que contiene los campos comunes para crear usuarios
/// </summary>
public abstract class BaseUsuarioDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public int IdTipoDocumento { get; set; }
    public string NroDocumento { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string Password { get; set; } = null!; // Se hasheará en el servicio
    public int IdRol { get; set; }
    public string? Genero { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public string? FotoPerfil { get; set; }
}
