using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    public interface IRepositoryUsuarios
    {
        Task CreateUsuarioAsync(string nombre, string password, string email, string? telefono, int rolId);
        Task<Usuario?> LogInUsuarioAsync(string email, string password);
        Task<List<Peluqueria>> GetPeluqueriasUsuarioAsync(int id);
    }
}
