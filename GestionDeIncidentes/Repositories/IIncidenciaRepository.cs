using SistemaIncidencias.Models;
using System.Collections.Generic;

namespace SistemaIncidencias.Repositories
{
    public interface IIncidenciaRepository
    {
        IEnumerable<Incidencia> ObtenerTodas();
        Incidencia ObtenerPorId(int id);
        void Agregar(Incidencia incidencia);
        void Actualizar(Incidencia incidencia);
        void Eliminar(int id);
        void Guardar();

        List<EstadisticaIncidencias> ObtenerEstadisticas();
    }
}
