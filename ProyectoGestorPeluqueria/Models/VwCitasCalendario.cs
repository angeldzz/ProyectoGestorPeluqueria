namespace ProyectoGestorPeluqueria.Models;

public partial class VwCitasCalendario
{
    public int CitaId { get; set; }

    public int? ClienteId { get; set; }

    public int? EmpleadoId { get; set; }

    public int? ServicioId { get; set; }

    public int? EstadoId { get; set; }

    public int PeluqueriaId { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime FechaHoraFin { get; set; }

    public string? NombreCliente { get; set; }

    public string? NombreEmpleado { get; set; }

    public string? NombreServicio { get; set; }

    public string? NombreEstado { get; set; }

    public string? NotasCliente { get; set; }
}
