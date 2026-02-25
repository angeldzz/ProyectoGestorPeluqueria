using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Servicio
{
    public int ServicioId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int DuracionMin { get; set; }

    public int? PeluqueriaId { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Peluqueria? Peluqueria { get; set; }
}
