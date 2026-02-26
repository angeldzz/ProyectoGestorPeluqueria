using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class HorariosEmpleado
{
    public int HorarioId { get; set; }

    public int EmpleadoId { get; set; }

    public DateTime FechaHoraApertura { get; set; }

    public DateTime FechaHoraCierre { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
}
