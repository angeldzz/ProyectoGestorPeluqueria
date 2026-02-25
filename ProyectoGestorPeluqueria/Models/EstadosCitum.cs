using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class EstadosCitum
{
    public int EstadoId { get; set; }

    public string NombreEstado { get; set; } = null!;

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
}
