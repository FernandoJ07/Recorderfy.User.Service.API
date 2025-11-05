using System;
using System.Collections.Generic;

namespace Recorderfy.User.Service.Model.Entities;

public partial class Medico : Usuario
{
    public string? Especialidad { get; set; }
    public string? CentroMedico { get; set; }
    public string? PacientesAsignados { get; set; }
    public bool? NotificacionesActivadas { get; set; }
    public string? FirmaDigital { get; set; }

    // Navegaciones a Pacientes (1:N)
    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}