using System;
using System.Collections.Generic;

namespace ProyectoGestorPeluqueria.Models;

public partial class VwPeluqueriaDuenoServicio
{
    public int PeluqueriaId { get; set; }

    public string NombrePeluqueria { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Coordenadas { get; set; }

    public string? UrlLogo { get; set; }

    public int DuenoId { get; set; }

    public string NombreDueno { get; set; } = null!;

    public string EmailDueno { get; set; } = null!;

    public string? TelefonoDueno { get; set; }

    public int? ServicioId { get; set; }

    public string? NombreServicio { get; set; }

    public decimal? Precio { get; set; }

    public int? DuracionMin { get; set; }
}
