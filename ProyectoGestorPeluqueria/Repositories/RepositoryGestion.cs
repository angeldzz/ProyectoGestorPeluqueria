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
------------------------------------------
    EXEC SP_INSERTAR_PELUQUERIA
    @Nombre = N'Nueva Imagen Barber',
    @Direccion = N'Avenida Siempre Viva 742',
    @UrlLogo = N'https://misitio.com/logo.png',
    @Coordenadas = N'40.4165,-3.7025',
    @PropietarioID = 2;
     */
    #endregion
    public class RepositoryGestion : IRepositoryGestion
    {
        private PeluqueriaContext context;

        public RepositoryGestion(PeluqueriaContext context)
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
    }
}
