using System;
using System.Collections.Generic;

namespace Recorderfy.User.Service.Model.Entities;

public partial class TipoDocumento
{
    public int IdTipoDocumento { get; set; }

    public string NombreDocumento { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
