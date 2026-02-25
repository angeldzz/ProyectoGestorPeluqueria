using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class HorariosEmpleado
{
    public int HorarioId { get; set; }

    public int? EmpleadoId { get; set; }

    public int? DiaSemana { get; set; }

    public TimeOnly HoraApertura { get; set; }

    public TimeOnly HoraCierre { get; set; }

    public virtual Empleado? Empleado { get; set; }
}
