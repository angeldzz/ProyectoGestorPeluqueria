using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Peluqueria
{
    public int PeluqueriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? UrlLogo { get; set; }

    public string? Coordenadas { get; set; }

    public int? PropietarioId { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    public virtual Usuario? Propietario { get; set; }

    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
