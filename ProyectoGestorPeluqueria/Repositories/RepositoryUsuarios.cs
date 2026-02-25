using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreCryptography.Helpers;
using ProyectoGestorPeluqueria.Data;
using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    public class RepositoryUsuarios
    {
        private PeluqueriaContext context;
        public RepositoryUsuarios(PeluqueriaContext context)
        {
            this.context = context;
        }

        public async Task CreateUsuarioAsync(string nombre, string password, string email, string? telefono, int rolId)
        {
            string salt = HelperTools.GenerateSalt();
            byte[] pass = HelperCryptography.EncryptPassword(password, salt);

            SqlParameter paramNombre   = new SqlParameter("@Nombre",   nombre);
            SqlParameter paramPassword = new SqlParameter("@Password", password);
            SqlParameter paramEmail    = new SqlParameter("@Email",    email);
            SqlParameter paramTelefono = new SqlParameter("@Telefono", (object?)telefono ?? DBNull.Value);
            SqlParameter paramRolId    = new SqlParameter("@RolID",    rolId);
            SqlParameter paramPass     = new SqlParameter("@Pass",     pass);
            SqlParameter paramSalt     = new SqlParameter("@Salt",     salt);

            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC SP_REGISTRAR_USUARIO @Nombre, @Password, @Email, @Telefono, @RolID, @Pass, @Salt",
                paramNombre, paramPassword, paramEmail, paramTelefono, paramRolId, paramPass, paramSalt);
        }

        public async Task<Usuario?> LogInUsuarioAsync(string email, string password)
        {
            var consulta = from datos in this.context.VwUsuariosCredenciales
                           where datos.Email == email
                           select datos;

            VwUsuariosCredenciale? credencial = await consulta.FirstOrDefaultAsync();
            if (credencial == null)
            {
                return null;
            }

            byte[] temp = HelperCryptography.EncryptPassword(password, credencial.Salt);
            bool response = HelperTools.CompareArrays(temp, credencial.Pass);

            if (response)
            {
                return await this.context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            }
            return null;
        }
    }
}
