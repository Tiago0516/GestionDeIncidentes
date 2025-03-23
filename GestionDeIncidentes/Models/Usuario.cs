using System.ComponentModel.DataAnnotations;

namespace SistemaIncidencias.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}
