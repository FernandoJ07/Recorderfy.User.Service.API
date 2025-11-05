using System;
using System.Collections.Generic;

namespace Recorderfy.User.Service.Model.Entities;

public partial class Cuidador : Usuario
{
    public string? RelacionConPaciente { get; set; }
    public string? Direccion { get; set; }
    public string? PacientesAsociados { get; set; }
    public bool? NotificacionesActivadas { get; set; }

    // Navegación a Pacientes (1:N)
    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
}