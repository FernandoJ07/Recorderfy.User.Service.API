using Microsoft.EntityFrameworkCore;
using Recorderfy.User.Service.Model.Entities;

namespace Recorderfy.User.Service.DAL.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuidador> Cuidadores { get; set; }
    public virtual DbSet<Medico> Medicos { get; set; }
    public virtual DbSet<Paciente> Pacientes { get; set; }
    public virtual DbSet<Rol> Roles { get; set; }
    public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        // Configuración de Usuario (Tabla Base con herencia TPT)
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");
            entity.ToTable("usuario");
            
            // TPT: Configurar herencia Table-Per-Type
            entity.UseTptMappingStrategy();

            entity.HasIndex(e => e.NroDocumento, "idx_usuarios_documento");
            entity.HasIndex(e => e.Email, "idx_usuarios_email");
            entity.HasIndex(e => e.Email, "usuarios_email_key").IsUnique();
            entity.HasIndex(e => e.NroDocumento, "usuarios_nro_documento_key").IsUnique();

            entity.Property(e => e.IdUsuario)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendiente'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.FotoPerfil).HasColumnName("foto_perfil");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .HasColumnName("genero");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdTipoDocumento).HasColumnName("id_tipo_documento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NroDocumento)
                .HasMaxLength(50)
                .HasColumnName("nro_documento");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .HasColumnName("telefono");
            entity.Property(e => e.UltimoAcceso)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ultimo_acceso");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("usuarios_id_rol_fkey");

            entity.HasOne(d => d.IdTipoDocumentoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipoDocumento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("usuarios_id_tipo_documento_fkey");
        });

        // Configuración de Cuidador (Hereda de Usuario)
        modelBuilder.Entity<Cuidador>(entity =>
        {
            entity.ToTable("cuidador");

            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.NotificacionesActivadas)
                .HasDefaultValue(true)
                .HasColumnName("notificaciones_activadas");
            entity.Property(e => e.PacientesAsociados)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("pacientes_asociados");
            entity.Property(e => e.RelacionConPaciente)
                .HasMaxLength(50)
                .HasColumnName("relacion_con_paciente");
        });

        // Configuración de Medico (Hereda de Usuario)
        modelBuilder.Entity<Medico>(entity =>
        {
            entity.ToTable("medico");

            entity.Property(e => e.CentroMedico)
                .HasMaxLength(150)
                .HasColumnName("centro_medico");
            entity.Property(e => e.Especialidad)
                .HasMaxLength(100)
                .HasColumnName("especialidad");
            entity.Property(e => e.FirmaDigital).HasColumnName("firma_digital");
            entity.Property(e => e.NotificacionesActivadas)
                .HasDefaultValue(true)
                .HasColumnName("notificaciones_activadas");
            entity.Property(e => e.PacientesAsignados)
                .HasDefaultValueSql("'[]'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("pacientes_asignados");
        });

        // Configuración de Paciente (Hereda de Usuario)
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("paciente");

            entity.HasIndex(e => e.IdCuidador, "idx_pacientes_cuidador");
            entity.HasIndex(e => e.IdMedico, "idx_pacientes_medico");

            entity.Property(e => e.DiagnosticoInicial)
                .HasMaxLength(150)
                .HasColumnName("diagnostico_inicial");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.FotoReferencia).HasColumnName("foto_referencia");
            entity.Property(e => e.IdCuidador).HasColumnName("id_cuidador");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.ObservacionesClinicas).HasColumnName("observaciones_clinicas");

            entity.HasOne(d => d.Cuidador).WithMany(p => p.Pacientes)
                .HasForeignKey(d => d.IdCuidador)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("paciente_id_cuidador_fkey");

            entity.HasOne(d => d.Medico).WithMany(p => p.Pacientes)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("paciente_id_medico_fkey");
        });

        // Configuración de Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("roles_pkey");
            entity.ToTable("rol");

            entity.HasIndex(e => e.NombreRol, "roles_nombre_rol_key").IsUnique();

            entity.Property(e => e.IdRol)
                .HasDefaultValueSql("nextval('roles_id_rol_seq'::regclass)")
                .HasColumnName("id_rol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .HasColumnName("nombre_rol");
        });

        // Configuración de TipoDocumento
        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento).HasName("tipos_documento_pkey");
            entity.ToTable("tipo_documento");

            entity.HasIndex(e => e.NombreDocumento, "tipos_documento_nombre_documento_key").IsUnique();

            entity.Property(e => e.IdTipoDocumento)
                .HasDefaultValueSql("nextval('tipos_documento_id_tipo_documento_seq'::regclass)")
                .HasColumnName("id_tipo_documento");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.NombreDocumento)
                .HasMaxLength(50)
                .HasColumnName("nombre_documento");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}