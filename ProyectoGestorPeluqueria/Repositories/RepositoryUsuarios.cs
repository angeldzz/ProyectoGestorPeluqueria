using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreCryptography.Helpers;
using ProyectoGestorPeluqueria.Data;
using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    #region STORED_PROCEDURES
    /*
     GO
CREATE OR ALTER PROCEDURE SP_REGISTRAR_USUARIO
    @Nombre    NVARCHAR(100),
    @Password  NVARCHAR(100),
    @Email     NVARCHAR(100),
    @Telefono  NVARCHAR(20)   = NULL,
    @RolID     INT,
    @Pass      VARBINARY(MAX),
    @Salt      NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el email no esté ya registrado
    IF EXISTS (SELECT 1 FROM dbo.USUARIOS WHERE email = @Email)
    BEGIN
        RAISERROR('El email "%s" ya está registrado.', 16, 1, @Email);
        RETURN;
    END

    -- Verificar que el RolID existe
    IF NOT EXISTS (SELECT 1 FROM dbo.ROLES WHERE RolID = @RolID)
    BEGIN
        RAISERROR('El RolID %d no existe.', 16, 1, @RolID);
        RETURN;
    END

    DECLARE @NuevoUsuarioID INT;

    BEGIN TRANSACTION;
    BEGIN TRY

        -- Generar nuevo UsuarioID
        SELECT @NuevoUsuarioID = ISNULL(MAX(UsuarioID), 0) + 1
        FROM dbo.USUARIOS;

        -- Insertar en USUARIOS
        INSERT INTO dbo.USUARIOS (UsuarioID, Nombre, Password, email, telefono, RolID)
        VALUES (@NuevoUsuarioID, @Nombre, @Password, @Email, @Telefono, @RolID);

        -- Insertar credenciales de seguridad
        INSERT INTO dbo.USUARIOS_SECURITY (UsuarioID, Pass, Salt)
        VALUES (@NuevoUsuarioID, @Pass, @Salt);

        COMMIT TRANSACTION;

        -- Devolver el nuevo usuario creado
        SELECT
            u.UsuarioID,
            u.Nombre,
            u.email,
            u.telefono,
            r.NombreRol
        FROM dbo.USUARIOS AS u
        INNER JOIN dbo.ROLES AS r ON r.RolID = u.RolID
        WHERE u.UsuarioID = @NuevoUsuarioID;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
     */
    #endregion
    public class RepositoryUsuarios : IRepositoryUsuarios
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
