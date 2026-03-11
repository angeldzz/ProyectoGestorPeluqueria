using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoGestorPeluqueria.Data;
using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Repositories
{
    #region STORED_PROCEDURES
    /*
     CREATE OR ALTER PROCEDURE SP_INSERTAR_PELUQUERIA
    @Nombre         NVARCHAR(100),
    @Direccion      NVARCHAR(255) = NULL,
    @UrlLogo        NVARCHAR(MAX) = NULL,
    @Coordenadas    NVARCHAR(100) = NULL,
    @PropietarioID  INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validar que el PropietarioID existe en la tabla USUARIOS
    IF NOT EXISTS (SELECT 1 FROM dbo.USUARIOS WHERE UsuarioID = @PropietarioID)
    BEGIN
        RAISERROR('Error: El PropietarioID %d no existe en la tabla de Usuarios.', 16, 1, @PropietarioID);
        RETURN;
    END

    -- 2. Validar que el nombre no sea nulo o vacío
    IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
    BEGIN
        RAISERROR('Error: El nombre de la peluquería es obligatorio.', 16, 1);
        RETURN;
    END

    DECLARE @NuevaPeluqueriaID INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 3. Calcular el nuevo PeluqueriaID (siguiendo tu lógica de negocio actual)
        SELECT @NuevaPeluqueriaID = ISNULL(MAX(PeluqueriaID), 0) + 1
        FROM dbo.PELUQUERIAS;

        -- 4. Insertar el registro
        INSERT INTO dbo.PELUQUERIAS (
            PeluqueriaID, 
            Nombre, 
            Direccion, 
            UrlLogo, 
            Coordenadas, 
            PropietarioID
        )
        VALUES (
            @NuevaPeluqueriaID, 
            @Nombre, 
            @Direccion, 
            @UrlLogo, 
            @Coordenadas, 
            @PropietarioID
        );

        COMMIT TRANSACTION;

        -- 5. Retornar los datos de la peluquería creada para confirmación
        SELECT * FROM dbo.PELUQUERIAS WHERE PeluqueriaID = @NuevaPeluqueriaID;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO


GO
CREATE OR ALTER PROCEDURE SP_INSERT_SERVICIO_PELUQUERIA
    @Nombre       NVARCHAR(100),
    @Precio       DECIMAL(10, 2),
    @DuracionMin  INT,
    @PeluqueriaID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que la peluquería existe
    IF NOT EXISTS (SELECT 1 FROM dbo.PELUQUERIAS WHERE PeluqueriaID = @PeluqueriaID)
    BEGIN
        RAISERROR('La PeluqueriaID %d no existe.', 16, 1, @PeluqueriaID);
        RETURN;
    END

    -- Verificar que no existe ya un servicio con el mismo nombre en esa peluquería
    IF EXISTS (SELECT 1 FROM dbo.SERVICIOS WHERE Nombre = @Nombre AND PeluqueriaID = @PeluqueriaID)
    BEGIN
        RAISERROR('Ya existe un servicio con el nombre "%s" en esta peluquería.', 16, 1, @Nombre);
        RETURN;
    END

    -- Validar Precio positivo
    IF @Precio <= 0
    BEGIN
        RAISERROR('El precio debe ser mayor que 0.', 16, 1);
        RETURN;
    END

    -- Validar DuracionMin positiva
    IF @DuracionMin <= 0
    BEGIN
        RAISERROR('La duración en minutos debe ser mayor que 0.', 16, 1);
        RETURN;
    END

    DECLARE @NuevoServicioID INT;

    BEGIN TRANSACTION;
    BEGIN TRY

        -- Generar nuevo ServicioID
        SELECT @NuevoServicioID = ISNULL(MAX(ServicioID), 0) + 1
        FROM dbo.SERVICIOS;

        -- Insertar el nuevo servicio
        INSERT INTO dbo.SERVICIOS (ServicioID, Nombre, Precio, DuracionMin, PeluqueriaID)
        VALUES (@NuevoServicioID, @Nombre, @Precio, @DuracionMin, @PeluqueriaID);

        COMMIT TRANSACTION;

        -- Devolver el servicio creado
        SELECT
            s.ServicioID,
            s.Nombre,
            s.Precio,
            s.DuracionMin,
            p.Nombre AS NombrePeluqueria
        FROM dbo.SERVICIOS         AS s
        INNER JOIN dbo.PELUQUERIAS AS p ON p.PeluqueriaID = s.PeluqueriaID
        WHERE s.ServicioID = @NuevoServicioID;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
    GO
CREATE OR ALTER PROCEDURE dbo.SP_ELIMINAR_PELUQUERIA
    @PeluqueriaID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que la peluquería existe
    IF NOT EXISTS (SELECT 1 FROM dbo.PELUQUERIAS WHERE PeluqueriaID = @PeluqueriaID)
    BEGIN
        RAISERROR('La PeluqueriaID %d no existe.', 16, 1, @PeluqueriaID);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY

        -- 1. Eliminar mensajes relacionados con la peluquería o sus citas
        DELETE FROM dbo.MENSAJES
        WHERE PeluqueriaID = @PeluqueriaID
           OR CitaID IN (
               SELECT c.CitaID
               FROM dbo.CITAS        AS c
               INNER JOIN dbo.EMPLEADOS AS e ON e.EmpleadoID = c.EmpleadoID
               WHERE e.PeluqueriaID = @PeluqueriaID
           );

        -- 2. Eliminar citas vinculadas a los empleados de la peluquería
        DELETE FROM dbo.CITAS
        WHERE EmpleadoID IN (
            SELECT EmpleadoID FROM dbo.EMPLEADOS WHERE PeluqueriaID = @PeluqueriaID
        );

        -- 3. Eliminar horarios de los empleados de la peluquería
        DELETE FROM dbo.HORARIOS_EMPLEADO
        WHERE EmpleadoID IN (
            SELECT EmpleadoID FROM dbo.EMPLEADOS WHERE PeluqueriaID = @PeluqueriaID
        );

        -- 4. Eliminar empleados de la peluquería
        DELETE FROM dbo.EMPLEADOS WHERE PeluqueriaID = @PeluqueriaID;

        -- 5. Eliminar servicios de la peluquería
        DELETE FROM dbo.SERVICIOS WHERE PeluqueriaID = @PeluqueriaID;

        -- 6. Eliminar la peluquería
        DELETE FROM dbo.PELUQUERIAS WHERE PeluqueriaID = @PeluqueriaID;

        COMMIT TRANSACTION;

        SELECT CONCAT('Peluquería con ID ', @PeluqueriaID, ' eliminada correctamente.') AS Resultado;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_ELIMINAR_SERVICIO
    @ServicioID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIOS WHERE ServicioID = @ServicioID)
    BEGIN
        RAISERROR('El ServicioID %d no existe.', 16, 1, @ServicioID);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY

        -- 1. Eliminar mensajes vinculados a citas de este servicio
        DELETE FROM dbo.MENSAJES
        WHERE CitaID IN (
            SELECT CitaID FROM dbo.CITAS WHERE ServicioID = @ServicioID
        );

        -- 2. Eliminar citas que usan este servicio
        DELETE FROM dbo.CITAS WHERE ServicioID = @ServicioID;

        -- 3. Eliminar el servicio
        DELETE FROM dbo.SERVICIOS WHERE ServicioID = @ServicioID;

        COMMIT TRANSACTION;

        SELECT CONCAT('Servicio con ID ', @ServicioID, ' eliminado correctamente.') AS Resultado;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

GO
CREATE OR ALTER PROCEDURE dbo.SP_ELIMINAR_HORARIO
    @HorarioID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.HORARIOS_EMPLEADO WHERE HorarioID = @HorarioID)
    BEGIN
        RAISERROR('El HorarioID %d no existe.', 16, 1, @HorarioID);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY

        DELETE FROM dbo.HORARIOS_EMPLEADO WHERE HorarioID = @HorarioID;

        COMMIT TRANSACTION;

        SELECT CONCAT('Horario con ID ', @HorarioID, ' eliminado correctamente.') AS Resultado;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_ELIMINAR_EMPLEADO
    @EmpleadoID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.EMPLEADOS WHERE EmpleadoID = @EmpleadoID)
    BEGIN
        RAISERROR('El EmpleadoID %d no existe.', 16, 1, @EmpleadoID);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY

        -- 1. Eliminar mensajes vinculados a citas de este empleado
        DELETE FROM dbo.MENSAJES
        WHERE CitaID IN (
            SELECT CitaID FROM dbo.CITAS WHERE EmpleadoID = @EmpleadoID
        );

        -- 2. Eliminar citas del empleado
        DELETE FROM dbo.CITAS WHERE EmpleadoID = @EmpleadoID;

        -- 3. Eliminar horarios del empleado
        DELETE FROM dbo.HORARIOS_EMPLEADO WHERE EmpleadoID = @EmpleadoID;

        -- 4. Eliminar el empleado
        DELETE FROM dbo.EMPLEADOS WHERE EmpleadoID = @EmpleadoID;

        COMMIT TRANSACTION;

        SELECT CONCAT('Empleado con ID ', @EmpleadoID, ' eliminado correctamente.') AS Resultado;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
------------------------------------------
     */
    #endregion
    public class RepositoryGestorPeluqueria : IRepositoryGestorPeluqueria
    {
        private PeluqueriaContext context;

        public RepositoryGestorPeluqueria(PeluqueriaContext context)
        {
            this.context = context;
        }

        public async Task CreatePeluqueria
            (string nombre, string? direccion,
            string? logo, string? cordenadas,
            int propietario)
        {
            SqlParameter paramNombre      = new SqlParameter("@Nombre",        nombre);
            SqlParameter paramDireccion   = new SqlParameter("@Direccion",     (object?)direccion  ?? DBNull.Value);
            SqlParameter paramLogo        = new SqlParameter("@UrlLogo",       (object?)logo        ?? DBNull.Value);
            SqlParameter paramCoordenadas = new SqlParameter("@Coordenadas",   (object?)cordenadas  ?? DBNull.Value);
            SqlParameter paramPropietario = new SqlParameter("@PropietarioID", propietario);

            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC SP_INSERTAR_PELUQUERIA @Nombre, @Direccion, @UrlLogo, @Coordenadas, @PropietarioID",
                paramNombre, paramDireccion, paramLogo, paramCoordenadas, paramPropietario);
        }
        public async Task CreateServicioPeluqueria(string nombre, decimal precio, int duracionMin, int idPeluqueria)
        {
            SqlParameter paramNombre      = new SqlParameter("@Nombre",       nombre);
            SqlParameter paramPrecio      = new SqlParameter("@Precio",       precio);
            SqlParameter paramDuracion    = new SqlParameter("@DuracionMin",  duracionMin);
            SqlParameter paramPeluqueria  = new SqlParameter("@PeluqueriaID", idPeluqueria);

            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC SP_INSERT_SERVICIO_PELUQUERIA @Nombre, @Precio, @DuracionMin, @PeluqueriaID",
                paramNombre, paramPrecio, paramDuracion, paramPeluqueria);
        }

        public async Task CreateEmpleadoPeluqueria(string nombre, int idPeluqueria)
        {
            SqlParameter paramNombre     = new SqlParameter("@Nombre",       nombre);
            SqlParameter paramPeluqueria = new SqlParameter("@PeluqueriaID", idPeluqueria);

            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC SP_INSERT_EMPLEADO @Nombre, @PeluqueriaID",
                paramNombre, paramPeluqueria);
        }
        public Task<List<Empleado>> FindEmpleadosPeluqueria(int idPeluqueria)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.PeluqueriaId == idPeluqueria
                           select datos;
            return consulta.ToListAsync();
        }

        public Task<VwPeluqueriaDuenoServicio> FindPeluqueria(int idPeluqueria)
        {
            var consulta = from datos in this.context.VwPeluqueriaDuenoServicios
                           where datos.PeluqueriaId == idPeluqueria
                           select datos;
            return consulta.FirstOrDefaultAsync();
        }

        public Task<List<Servicio>> FindServiciosPeluqueria(int idPeluqueria)
        {
            var consulta = from datos in this.context.Servicios
                           where datos.PeluqueriaId == idPeluqueria
                           select datos;
            return consulta.ToListAsync();
        }

        public async Task<List<VwPeluqueriaDuenoServicio>> MostrarPeluquerias()
        {
            return await this.context.VwPeluqueriaDuenoServicios.ToListAsync();
        }

        public Task<Servicio?> FindServicio(int servicioId)
        {
            return this.context.Servicios.FindAsync(servicioId).AsTask();
        }

        public Task<List<VwCitasCalendario>> GetCitasCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId)
        {
            var query = this.context.VwCitasCalendarios
                .Where(c => c.PeluqueriaId == peluqueriaId
                         && c.FechaHoraInicio < end
                         && c.FechaHoraFin > start);

            if (empleadoId.HasValue)
                query = query.Where(c => c.EmpleadoId == empleadoId.Value);

            return query.ToListAsync();
        }

        public Task<List<HorariosEmpleado>> GetHorariosCalendario(int peluqueriaId, DateTime start, DateTime end, int? empleadoId)
        {
            var empleadosIds = this.context.Empleados
                .Where(e => e.PeluqueriaId == peluqueriaId)
                .Select(e => e.EmpleadoId);

            var query = this.context.HorariosEmpleados
                .Include(h => h.Empleado)
                .Where(h => empleadosIds.Contains(h.EmpleadoId)
                         && h.FechaHoraApertura < end
                         && h.FechaHoraCierre > start);

            if (empleadoId.HasValue)
                query = query.Where(h => h.EmpleadoId == empleadoId.Value);

            return query.ToListAsync();
        }

        public async Task<bool> EmpleadoDisponible(int empleadoId, DateTime inicio, DateTime fin)
        {
            bool tieneHorario = await this.context.HorariosEmpleados
                .AnyAsync(h => h.EmpleadoId == empleadoId
                            && h.FechaHoraApertura <= inicio
                            && h.FechaHoraCierre >= fin);
            if (!tieneHorario) return false;

            bool tieneSolape = await this.context.Citas
                .AnyAsync(c => c.EmpleadoId == empleadoId
                            && c.EstadoId != 3
                            && c.EstadoId != 4
                            && c.FechaHoraInicio < fin
                            && c.FechaHoraFin > inicio);
            return !tieneSolape;
        }

        public async Task CrearCita(int clienteId, int empleadoId, int servicioId, DateTime inicio, string? notas)
        {
            var servicio = await this.context.Servicios.FindAsync(servicioId)
                ?? throw new ArgumentException($"El servicio con ID '{servicioId}' no existe.", nameof(servicioId));

            var fin = inicio.AddMinutes(servicio.DuracionMin);

            if (!await EmpleadoDisponible(empleadoId, inicio, fin))
                throw new InvalidOperationException("El empleado no está disponible en el horario seleccionado.");

            int newId = (await this.context.Citas.MaxAsync(c => (int?)c.CitaId) ?? 0) + 1;

            this.context.Citas.Add(new Cita
            {
                CitaId = newId,
                ClienteId = clienteId,
                EmpleadoId = empleadoId,
                ServicioId = servicioId,
                FechaHoraInicio = inicio,
                FechaHoraFin = fin,
                NotasCliente = notas,
                EstadoId = 1
            });
            await this.context.SaveChangesAsync();
        }

        public async Task CambiarEstadoCita(int citaId, int estadoId)
        {
            bool estadoExists = await this.context.EstadosCita
                .AnyAsync(e => e.EstadoId == estadoId);

            if (!estadoExists)
                throw new ArgumentException($"EstadoId '{estadoId}' does not exist.", nameof(estadoId));

            var cita = await this.context.Citas.FindAsync(citaId);
            if (cita != null)
            {
                cita.EstadoId = estadoId;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task AgregarHorario(int empleadoId, DateTime apertura, DateTime cierre)
        {
            this.context.HorariosEmpleados.Add(new HorariosEmpleado
            {
                EmpleadoId = empleadoId,
                FechaHoraApertura = apertura,
                FechaHoraCierre = cierre
            });
            await this.context.SaveChangesAsync();
        }

        public async Task DeletePeluqueriaAsync(int idPeluqueria)
        {
            SqlParameter paramPeluqueria = new SqlParameter("@PeluqueriaID", idPeluqueria);
            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_ELIMINAR_PELUQUERIA @PeluqueriaID",
                paramPeluqueria);
        }

        public async Task DeleteServicioAsync(int servicioId)
        {
            SqlParameter paramServicio = new SqlParameter("@ServicioID", servicioId);
            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_ELIMINAR_SERVICIO @ServicioID",
                paramServicio);
        }

        public async Task DeleteHorarioAsync(int horarioId)
        {
            SqlParameter paramHorario = new SqlParameter("@HorarioID", horarioId);
            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_ELIMINAR_HORARIO @HorarioID",
                paramHorario);
        }

        public async Task DeleteEmpleadoAsync(int empleadoId)
        {
            SqlParameter paramEmpleado = new SqlParameter("@EmpleadoID", empleadoId);
            await this.context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_ELIMINAR_EMPLEADO @EmpleadoID",
                paramEmpleado);
        }

        public Task<List<VwCitasCalendario>> GetCitasClienteAsync(int clienteId, DateTime start, DateTime end)
        {
            return this.context.VwCitasCalendarios
                .Where(c => c.ClienteId == clienteId
                         && c.FechaHoraInicio < end
                         && c.FechaHoraFin > start)
                .OrderBy(c => c.FechaHoraInicio)
                .ToListAsync();
        }
    }
}
