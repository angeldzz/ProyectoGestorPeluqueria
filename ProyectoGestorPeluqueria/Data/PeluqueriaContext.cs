using Microsoft.EntityFrameworkCore;
using ProyectoGestorPeluqueria.Models;

namespace ProyectoGestorPeluqueria.Data
{
    public class PeluqueriaContext: DbContext
    {
        public PeluqueriaContext(DbContextOptions<PeluqueriaContext> options)
            : base(options)
        {}

        public virtual DbSet<Cita> Citas { get; set; }

        public virtual DbSet<Empleado> Empleados { get; set; }

        public virtual DbSet<EstadosCitum> EstadosCita { get; set; }

        public virtual DbSet<HorariosEmpleado> HorariosEmpleados { get; set; }

        public virtual DbSet<Mensaje> Mensajes { get; set; }

        public virtual DbSet<Peluqueria> Peluquerias { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Servicio> Servicios { get; set; }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<UsuariosSecurity> UsuariosSecurity { get; set; }

        public virtual DbSet<VwMensajesDetalle> VwMensajesDetalles { get; set; }

        public virtual DbSet<VwUsuariosCredenciale> VwUsuariosCredenciales { get; set; }

        public virtual DbSet<VwPeluqueriaDuenoServicio> VwPeluqueriaDuenoServicios { get; set; }

        public virtual DbSet<VwCitasCalendario> VwCitasCalendarios { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstadosCitum>(entity =>
            {
                entity.HasKey(e => e.EstadoId);
                entity.ToTable("ESTADOS_CITA");
            });

            modelBuilder.Entity<HorariosEmpleado>(entity =>
            {
                entity.HasKey(e => e.HorarioId);
                entity.ToTable("HORARIOS_EMPLEADO");
                entity.Property(e => e.HorarioId).HasColumnName("HorarioID");
                entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
                entity.Property(e => e.FechaHoraApertura).HasColumnType("datetime");
                entity.Property(e => e.FechaHoraCierre).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RolId);
                entity.ToTable("ROLES");
            });

            modelBuilder.Entity<UsuariosSecurity>(entity =>
            {
                entity.HasKey(e => e.UsuarioId);
                entity.ToTable("USUARIOS_SECURITY");
            });

            modelBuilder.Entity<VwMensajesDetalle>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_MENSAJES_DETALLE");
            });

            modelBuilder.Entity<VwUsuariosCredenciale>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_USUARIOS_CREDENCIALES");
            });

            modelBuilder.Entity<VwPeluqueriaDuenoServicio>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_PELUQUERIA_DUENO_SERVICIOS");
            });

            modelBuilder.Entity<VwCitasCalendario>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("VW_CITAS_CALENDARIO");
            });
        }
    }
}

