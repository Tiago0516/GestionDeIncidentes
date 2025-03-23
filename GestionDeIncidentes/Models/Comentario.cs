using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaIncidencias.Models
{
    /// <summary>
    /// Clase que representa un comentario asociado a una incidencia
    /// </summary>
    public class Comentario
    {
        /// <summary>
        /// Identificador único del comentario
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Contenido del comentario
        /// </summary>
        [Required]
        public string Texto { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el comentario
        /// </summary>
        public DateTime Fecha { get; set; } = DateTime.Now;

        /// <summary>
        /// ID de la incidencia a la que pertenece este comentario
        /// </summary>
        [ForeignKey("Incidencia")]
        public int IncidenciaId { get; set; }

        /// <summary>
        /// Referencia a la incidencia asociada
        /// </summary>
        public virtual Incidencia Incidencia { get; set; }
    }
}
