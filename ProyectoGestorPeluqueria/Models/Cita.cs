using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Cita
{
    public int CitaId { get; set; }

    public int? ClienteId { get; set; }

    public int? EmpleadoId { get; set; }

    public int? ServicioId { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime FechaHoraFin { get; set; }

    public string? NotasCliente { get; set; }

    public int? EstadoId { get; set; }

    public virtual Usuario? Cliente { get; set; }

    public virtual Empleado? Empleado { get; set; }

    public virtual EstadosCitum? Estado { get; set; }

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    public virtual Servicio? Servicio { get; set; }
}
