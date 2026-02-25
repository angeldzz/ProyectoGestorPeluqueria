using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Mensaje
{
    public int MensajeId { get; set; }

    public int? UsuarioId { get; set; }

    public int? PeluqueriaId { get; set; }

    public int? CitaId { get; set; }

    public string Mensaje1 { get; set; } = null!;

    public DateTime? HoraCreacion { get; set; }

    public virtual Cita? Cita { get; set; }

    public virtual Peluqueria? Peluqueria { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
