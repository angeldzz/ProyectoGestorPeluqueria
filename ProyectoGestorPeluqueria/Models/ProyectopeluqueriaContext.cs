using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoGestorPeluqueria.Models;

public partial class ProyectopeluqueriaContext : DbContext
{
    public ProyectopeluqueriaContext()
    {
    }

    public ProyectopeluqueriaContext(DbContextOptions<ProyectopeluqueriaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EstadosCitum> EstadosCita { get; set; }

    public virtual DbSet<HorariosEmpleado> HorariosEmpleados { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<Peluqueria> Peluquerias { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosSecurity> UsuariosSecurities { get; set; }

    public virtual DbSet<VwMensajesDetalle> VwMensajesDetalles { get; set; }

    public virtual DbSet<VwPeluqueriaDuenoServicio> VwPeluqueriaDuenoServicios { get; set; }

    public virtual DbSet<VwUsuariosCredenciale> VwUsuariosCredenciales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\DEVELOPER;Database=PROYECTOPELUQUERIA;User ID=sa;Password=;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.CitaId).HasName("PK__CITAS__F0E2D9F279E3136D");

            entity.ToTable("CITAS");

            entity.Property(e => e.CitaId)
                .ValueGeneratedNever()
                .HasColumnName("CitaID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.EstadoId).HasColumnName("EstadoID");
            entity.Property(e => e.ServicioId).HasColumnName("ServicioID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK_Citas_Clientes");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Cita)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_Citas_Empleados");

            entity.HasOne(d => d.Estado).WithMany(p => p.Cita)
                .HasForeignKey(d => d.EstadoId)
                .HasConstraintName("FK_Citas_Estados");

            entity.HasOne(d => d.Servicio).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ServicioId)
                .HasConstraintName("FK_Citas_Servicios");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.EmpleadoId).HasName("PK__EMPLEADO__958BE6F0107E1D64");

            entity.ToTable("EMPLEADOS");

            entity.Property(e => e.EmpleadoId)
                .ValueGeneratedNever()
                .HasColumnName("EmpleadoID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PeluqueriaId).HasColumnName("PeluqueriaID");

            entity.HasOne(d => d.Peluqueria).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.PeluqueriaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Empleados_Peluquerias");
        });

        modelBuilder.Entity<EstadosCitum>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__ESTADOS___FEF86B6053712EAA");

            entity.ToTable("ESTADOS_CITA");

            entity.Property(e => e.EstadoId)
                .ValueGeneratedNever()
                .HasColumnName("EstadoID");
            entity.Property(e => e.NombreEstado).HasMaxLength(50);
        });

        modelBuilder.Entity<HorariosEmpleado>(entity =>
        {
            entity.HasKey(e => e.HorarioId).HasName("PK_Horarios");

            entity.ToTable("HORARIOS_EMPLEADO");

            entity.Property(e => e.HorarioId).HasColumnName("HorarioID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.FechaHoraApertura).HasColumnType("datetime");
            entity.Property(e => e.FechaHoraCierre).HasColumnType("datetime");

            entity.HasOne(d => d.Empleado).WithMany(p => p.HorariosEmpleados)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_Horarios_Empleados");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.MensajeId).HasName("PK__MENSAJES__FEA0557FA3EA0C90");

            entity.ToTable("MENSAJES");

            entity.Property(e => e.MensajeId)
                .ValueGeneratedNever()
                .HasColumnName("MensajeID");
            entity.Property(e => e.CitaId).HasColumnName("CitaID");
            entity.Property(e => e.HoraCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Mensaje1).HasColumnName("Mensaje");
            entity.Property(e => e.PeluqueriaId).HasColumnName("PeluqueriaID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Cita).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.CitaId)
                .HasConstraintName("FK_Mensajes_Citas");

            entity.HasOne(d => d.Peluqueria).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.PeluqueriaId)
                .HasConstraintName("FK_Mensajes_Peluquerias");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_Mensajes_Usuarios");
        });

        modelBuilder.Entity<Peluqueria>(entity =>
        {
            entity.HasKey(e => e.PeluqueriaId).HasName("PK__PELUQUER__DBB815D1B3DCD8A6");

            entity.ToTable("PELUQUERIAS");

            entity.Property(e => e.PeluqueriaId)
                .ValueGeneratedNever()
                .HasColumnName("PeluqueriaID");
            entity.Property(e => e.Coordenadas).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PropietarioId).HasColumnName("PropietarioID");

            entity.HasOne(d => d.Propietario).WithMany(p => p.Peluqueria)
                .HasForeignKey(d => d.PropietarioId)
                .HasConstraintName("FK_Peluquerias_Usuarios");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__ROLES__F92302D1AAB75F5E");

            entity.ToTable("ROLES");

            entity.Property(e => e.RolId)
                .ValueGeneratedNever()
                .HasColumnName("RolID");
            entity.Property(e => e.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.ServicioId).HasName("PK__SERVICIO__D5AEEC2221D53AFF");

            entity.ToTable("SERVICIOS");

            entity.Property(e => e.ServicioId)
                .ValueGeneratedNever()
                .HasColumnName("ServicioID");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PeluqueriaId).HasColumnName("PeluqueriaID");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Peluqueria).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.PeluqueriaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Servicios_Peluquerias");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__USUARIOS__2B3DE7989A2A6540");

            entity.ToTable("USUARIOS");

            entity.HasIndex(e => e.Email, "UQ__USUARIOS__AB6E6164D200C789").IsUnique();

            entity.Property(e => e.UsuarioId)
                .ValueGeneratedNever()
                .HasColumnName("UsuarioID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<UsuariosSecurity>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__USUARIOS__2B3DE7983489979B");

            entity.ToTable("USUARIOS_SECURITY");

            entity.Property(e => e.UsuarioId)
                .ValueGeneratedNever()
                .HasColumnName("UsuarioID");
            entity.Property(e => e.Salt).HasMaxLength(50);

            entity.HasOne(d => d.Usuario).WithOne(p => p.UsuariosSecurity)
                .HasForeignKey<UsuariosSecurity>(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuariosSecurity_Usuarios");
        });

        modelBuilder.Entity<VwMensajesDetalle>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_MENSAJES_DETALLE");

            entity.Property(e => e.CitaId).HasColumnName("CitaID");
            entity.Property(e => e.DireccionPeluqueria).HasMaxLength(255);
            entity.Property(e => e.EmailUsuario).HasMaxLength(100);
            entity.Property(e => e.EstadoCita).HasMaxLength(50);
            entity.Property(e => e.MensajeId).HasColumnName("MensajeID");
            entity.Property(e => e.NombrePeluqueria).HasMaxLength(100);
            entity.Property(e => e.NombreUsuario).HasMaxLength(100);
        });

        modelBuilder.Entity<VwPeluqueriaDuenoServicio>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_PELUQUERIA_DUENO_SERVICIOS");

            entity.Property(e => e.Coordenadas).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.DuenoId).HasColumnName("DuenoID");
            entity.Property(e => e.EmailDueno).HasMaxLength(100);
            entity.Property(e => e.NombreDueno).HasMaxLength(100);
            entity.Property(e => e.NombrePeluqueria).HasMaxLength(100);
            entity.Property(e => e.NombreServicio).HasMaxLength(100);
            entity.Property(e => e.PeluqueriaId).HasColumnName("PeluqueriaID");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServicioId).HasColumnName("ServicioID");
            entity.Property(e => e.TelefonoDueno).HasMaxLength(20);
        });

        modelBuilder.Entity<VwUsuariosCredenciale>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_USUARIOS_CREDENCIALES");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.NombreRol).HasMaxLength(50);
            entity.Property(e => e.Salt).HasMaxLength(50);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
