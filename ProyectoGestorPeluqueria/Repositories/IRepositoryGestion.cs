using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    public interface IRepositoryGestion
    {
        Task CreatePeluqueria(string nombre, string? direccion, string? logo, string? cordenadas, int propietario);
        Task<List<VwPeluqueriaDuenoServicio>> MostrarPeluquerias();
        Task<List<Empleado>> FindEmpleadosPeluqueria(int idPeluqueria);
        Task<VwPeluqueriaDuenoServicio> FindPeluqueria(int idPeluqueria);
        Task<List<Servicio>> FindServiciosPeluqueria(int idPeluqueria);
        Task<Servicio?> FindServicio(int servicioId);

        Task<List<VwCitasCalendario>> GetCitasCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId);
        Task<List<HorariosEmpleado>> GetHorariosCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId);
        Task<bool> EmpleadoDisponible(int empleadoId, DateTime inicio, DateTime fin);
        Task CrearCita(int clienteId, int empleadoId, int servicioId, DateTime inicio, string? notas);
        Task CambiarEstadoCita(int citaId, int estadoId);
        Task AgregarHorario(int empleadoId, DateTime apertura, DateTime cierre);
    }
}
