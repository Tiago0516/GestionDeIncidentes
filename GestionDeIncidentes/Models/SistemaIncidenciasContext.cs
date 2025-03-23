// Importaciones necesarias
using System.Collections.Generic;
using System.Data.Entity;

namespace SistemaIncidencias.Models
{
    /// <summary>
    /// Clase que representa el contexto de la base de datos del sistema de incidencias
    /// </summary>
    public class SistemaIncidenciasContext : DbContext
    {
        /// <summary>
        /// Constructor que inicializa la conexión a la base de datos
        /// </summary>
        public SistemaIncidenciasContext() : base("name=SistemaIncidenciasDB")
        {
            // Deshabilita la inicialización automática de la base de datos
            Database.SetInitializer<SistemaIncidenciasContext>(null);
        }

        /// <summary>
        /// DbSet para la tabla de Incidencias
        /// </summary>
        public DbSet<Incidencia> Incidencias { get; set; }

        /// <summary>
        /// DbSet para la tabla de Usuarios
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// DbSet para la tabla de Técnicos
        /// </summary>
        public DbSet<Tecnico> Tecnicos { get; set; }

        /// <summary>
        /// DbSet para la tabla de Comentarios
        /// </summary>
        public DbSet<Comentario> Comentarios { get; set; }

        /// <summary>
        /// Método que configura el modelo de la base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la relación entre Incidencia y Usuario (reporta)
            // Un usuario puede reportar múltiples incidencias, pero una incidencia solo puede ser reportada por un usuario
            modelBuilder.Entity<Incidencia>()
                .HasRequired(i => i.UsuarioReporta)
                .WithMany()
                .HasForeignKey(i => i.UsuarioReportaId)
                .WillCascadeOnDelete(false);

            // Configurar la relación entre Incidencia y Tecnico (asignado)
            // Un técnico puede tener múltiples incidencias asignadas, pero una incidencia solo puede estar asignada a un técnico
            modelBuilder.Entity<Incidencia>()
                .HasOptional(i => i.TecnicoAsignado)
                .WithMany()
                .HasForeignKey(i => i.TecnicoAsignadoId)
                .WillCascadeOnDelete(false);
        }
    }
}
