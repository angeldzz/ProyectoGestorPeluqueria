using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class UsuariosSecurity
{
    public int UsuarioId { get; set; }

    public byte[] Pass { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
