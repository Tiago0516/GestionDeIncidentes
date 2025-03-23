// Importaciones necesarias del sistema
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaIncidencias.Models
{
    /// <summary>
    /// Clase que almacena estadísticas de incidencias agrupadas por categoría
    /// </summary>
    public class EstadisticaIncidencias
    {
        /// <summary>
        /// Representa la categoría de agrupación (Estado o Prioridad)
        /// </summary>
        public string Categoria { get; set; } // Puede ser Estado o Prioridad

        /// <summary>
        /// Almacena la cantidad de incidencias para la categoría
        /// </summary>
        public int Cantidad { get; set; }
    }
}