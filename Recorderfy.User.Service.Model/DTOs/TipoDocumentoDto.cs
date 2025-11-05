namespace Recorderfy.User.Service.Model.DTOs;

public class TipoDocumentoDto
{
    public int IdTipoDocumento { get; set; }
    public string NombreDocumento { get; set; } = null!;
    public string? Descripcion { get; set; }
}
