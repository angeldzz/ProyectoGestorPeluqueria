using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class VwMensajesDetalle
{
    public int MensajeId { get; set; }

    public DateTime? HoraCreacion { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string EmailUsuario { get; set; } = null!;

    public string NombrePeluqueria { get; set; } = null!;

    public string? DireccionPeluqueria { get; set; }

    public int? CitaId { get; set; }

    public DateTime? CitaFechaInicio { get; set; }

    public DateTime? CitaFechaFin { get; set; }

    public string? EstadoCita { get; set; }

    public string Mensaje { get; set; } = null!;
}
