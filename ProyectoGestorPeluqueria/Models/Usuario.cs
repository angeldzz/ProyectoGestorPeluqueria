using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public int? RolId { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    public virtual ICollection<Peluqueria> Peluqueria { get; set; } = new List<Peluqueria>();

    public virtual Role? Rol { get; set; }

    public virtual UsuariosSecurity? UsuariosSecurity { get; set; }
}
