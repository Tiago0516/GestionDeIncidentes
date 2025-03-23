using System.ComponentModel.DataAnnotations;

namespace SistemaIncidencias.Models
{
    public class Tecnico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }
    }
}
