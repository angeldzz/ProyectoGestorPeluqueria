using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Empleado
{
    public int EmpleadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Activo { get; set; }

    public int? PeluqueriaId { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<HorariosEmpleado> HorariosEmpleados { get; set; } = new List<HorariosEmpleado>();

    public virtual Peluqueria? Peluqueria { get; set; }
}
