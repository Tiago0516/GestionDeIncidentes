// Importamos las anotaciones de datos necesarias
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaIncidencias.Models
{
    // Esta clase representa la tabla "Tecnicos" en la base de datos
    [Table("Tecnicos")]
    public class Tecnico
    {
        // Identificador único del técnico
        [Key]
        public int Id { get; set; }

        // Nombre del técnico (campo requerido)
        [Required]
        public string Nombre { get; set; }
    }
} 