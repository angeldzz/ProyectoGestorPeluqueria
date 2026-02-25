using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class VwUsuariosCredenciale
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NombreRol { get; set; } = null!;

    public byte[] Pass { get; set; } = null!;

    public string Salt { get; set; } = null!;
}
