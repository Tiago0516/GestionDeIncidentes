// Importaciones necesarias del sistema
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaIncidencias.Models
{
    /// <summary>
    /// Clase que representa una incidencia en el sistema
    /// </summary>
    public class Incidencia
    {
        /// <summary>
        /// Identificador único de la incidencia
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Título descriptivo de la incidencia
        /// </summary>
        [Required]
        public string Titulo { get; set; }

        /// <summary>
        /// Descripción detallada de la incidencia
        /// </summary>
        [Required]
        public string Descripcion { get; set; }

        /// <summary>
        /// Estado actual de la incidencia (ej: Abierta, En Proceso, Cerrada)
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Nivel de prioridad de la incidencia (ej: Alta, Media, Baja)
        /// </summary>
        public string Prioridad { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó la incidencia
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha y hora de la última actualización de la incidencia
        /// </summary>
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        /// <summary>
        /// ID del usuario que reportó la incidencia
        /// </summary>
        [Required]
        [ForeignKey("UsuarioReporta")]
        public int UsuarioReportaId { get; set; }

        /// <summary>
        /// Usuario que reportó la incidencia
        /// </summary>
        public virtual Usuario UsuarioReporta { get; set; }

        /// <summary>
        /// ID del técnico asignado a la incidencia
        /// </summary>
        [ForeignKey("TecnicoAsignado")]
        public int? TecnicoAsignadoId { get; set; }

        /// <summary>
        /// Técnico asignado para resolver la incidencia
        /// </summary>
        public virtual Tecnico TecnicoAsignado { get; set; }

        /// <summary>
        /// Colección de comentarios asociados a la incidencia
        /// </summary>
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}
