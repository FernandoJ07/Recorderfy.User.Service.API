using System;
using System.Collections.Generic;

namespace Recorderfy.User.Service.Model.Entities;

public partial class Paciente : Usuario
{
    public string? DiagnosticoInicial { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public Guid? IdCuidador { get; set; }
    public Guid? IdMedico { get; set; }
    public string? ObservacionesClinicas { get; set; }
    public string? FotoReferencia { get; set; }

    // Navegaciones a entidades relacionadas
    public virtual Cuidador? Cuidador { get; set; }
    public virtual Medico? Medico { get; set; }
}