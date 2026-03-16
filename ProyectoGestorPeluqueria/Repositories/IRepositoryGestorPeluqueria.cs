using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    public interface IRepositoryGestorPeluqueria
    {
        // Peluqueria
        Task CreatePeluqueria(string nombre, string? direccion, string? logo, string? cordenadas, int propietario);
        Task CreateServicioPeluqueria(string nombre, decimal precio, int duracionMin, int idPeluqueria);
        Task CreateEmpleadoPeluqueria(string nombre, int idPeluqueria);
        Task<List<VwPeluqueriaDuenoServicio>> MostrarPeluquerias();
        Task<List<Empleado>> FindEmpleadosPeluqueria(int idPeluqueria);
        Task<VwPeluqueriaDuenoServicio> FindPeluqueria(int idPeluqueria);
        Task<List<Servicio>> FindServiciosPeluqueria(int idPeluqueria);
        Task<Servicio?> FindServicio(int servicioId);

        // Calendario
        Task<List<VwCitasCalendario>> GetCitasCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId);
        Task<List<HorariosEmpleado>> GetHorariosCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId);
        Task<bool> EmpleadoDisponible(int empleadoId, DateTime inicio, DateTime fin);
        Task CrearCita(int clienteId, int empleadoId, int servicioId, DateTime inicio, string? notas);
        Task CambiarEstadoCita(int citaId, int estadoId);
        Task AgregarHorario(int empleadoId, DateTime apertura, DateTime cierre);
        Task<List<Mensaje>> GetMensajesUsuarioAsync(int clienteId, int idPeluqueria);
        Task<List<Mensaje>> GetMensajesPeluqueriaAsync(int idPeluqueria);
        Task<List<Mensaje>> GetMensajesConversacionAsync(int idPeluqueria, int usuarioAId, int usuarioBId);
        Task CreateMensajeAsync(int usuarioId, int peluqueriaId, string mensajeText);
        Task<bool> DeleteMensajeAsync(int mensajeId, int usuarioId);
        
        // Deletes
        Task DeletePeluqueriaAsync(int idPeluqueria);
        Task DeleteServicioAsync(int servicioId);
        Task DeleteHorarioAsync(int horarioId);
        Task DeleteEmpleadoAsync(int empleadoId);

        // Cliente
        Task<List<VwCitasCalendario>> GetCitasClienteAsync(int clienteId, DateTime start, DateTime end);
    }
}
