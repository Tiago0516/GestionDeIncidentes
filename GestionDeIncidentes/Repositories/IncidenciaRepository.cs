// Importación de espacios de nombres necesarios
using SistemaIncidencias.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using log4net;

namespace SistemaIncidencias.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de incidencias
    /// </summary>
    public class IncidenciaRepository : IIncidenciaRepository
    {
        // Contexto de base de datos
        private readonly SistemaIncidenciasContext _context;
        // Logger para registro de eventos
        private static readonly ILog _logger = LogManager.GetLogger(typeof(IncidenciaRepository));

        /// <summary>
        /// Constructor que inicializa el repositorio con un contexto de base de datos
        /// </summary>
        public IncidenciaRepository(SistemaIncidenciasContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las incidencias con sus relaciones
        /// </summary>
        public IEnumerable<Incidencia> ObtenerTodas()
        {
            try
            {
                _logger.Info("Obteniendo todas las incidencias");
                return _context.Incidencias
                    .Include(i => i.UsuarioReporta)
                    .Include(i => i.TecnicoAsignado)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener todas las incidencias: {ex.Message}", ex);
                throw new RepositoryException("Error al recuperar las incidencias", ex);
            }
        }

        /// <summary>
        /// Obtiene una incidencia específica por su ID
        /// </summary>
        public Incidencia ObtenerPorId(int id)
        {
            try
            {
                _logger.Info($"Obteniendo incidencia con ID: {id}");
                var incidencia = _context.Incidencias
                    .Include(i => i.UsuarioReporta)
                    .Include(i => i.TecnicoAsignado)
                    .Include(i => i.Comentarios)
                    .FirstOrDefault(i => i.Id == id);

                if (incidencia == null)
                {
                    _logger.Warn($"No se encontró la incidencia con ID: {id}");
                    throw new IncidenciaNotFoundException($"No se encontró la incidencia con ID {id}");
                }

                return incidencia;
            }
            catch (Exception ex) when (!(ex is IncidenciaNotFoundException))
            {
                _logger.Error($"Error al obtener la incidencia {id}: {ex.Message}", ex);
                throw new RepositoryException($"Error al recuperar la incidencia {id}", ex);
            }
        }

        /// <summary>
        /// Agrega una nueva incidencia al repositorio
        /// </summary>
        public void Agregar(Incidencia incidencia)
        {
            try
            {
                if (incidencia == null)
                {
                    throw new ArgumentNullException(nameof(incidencia));
                }

                _logger.Info($"Agregando nueva incidencia: {incidencia.Titulo}");
                _context.Incidencias.Add(incidencia);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al agregar la incidencia: {ex.Message}", ex);
                throw new RepositoryException("Error al agregar la incidencia", ex);
            }
        }

        /// <summary>
        /// Actualiza una incidencia existente
        /// </summary>
        public void Actualizar(Incidencia incidencia)
        {
            try
            {
                if (incidencia == null)
                {
                    throw new ArgumentNullException(nameof(incidencia));
                }

                _logger.Info($"Actualizando incidencia ID {incidencia.Id}: {incidencia.Titulo}");
                _context.Entry(incidencia).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al actualizar la incidencia {incidencia.Id}: {ex.Message}", ex);
                throw new RepositoryException($"Error al actualizar la incidencia {incidencia.Id}", ex);
            }
        }

        /// <summary>
        /// Elimina una incidencia por su ID
        /// </summary>
        public void Eliminar(int id)
        {
            try
            {
                _logger.Info($"Eliminando incidencia con ID: {id}");
                var incidencia = _context.Incidencias.Find(id);
                if (incidencia == null)
                {
                    _logger.Warn($"Intento de eliminar una incidencia inexistente con ID: {id}");
                    throw new IncidenciaNotFoundException($"No se encontró la incidencia con ID {id} para eliminar");
                }

                _context.Incidencias.Remove(incidencia);
            }
            catch (Exception ex) when (!(ex is IncidenciaNotFoundException))
            {
                _logger.Error($"Error al eliminar la incidencia {id}: {ex.Message}", ex);
                throw new RepositoryException($"Error al eliminar la incidencia {id}", ex);
            }
        }

        /// <summary>
        /// Guarda todos los cambios pendientes en la base de datos
        /// </summary>
        public void Guardar()
        {
            try
            {
                _logger.Info("Guardando cambios en la base de datos");
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.Error($"Error de concurrencia al guardar cambios: {ex.Message}", ex);
                throw new ConcurrencyException("Los datos han sido modificados por otro usuario", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Error de base de datos al guardar cambios: {ex.Message}", ex);
                throw new RepositoryException("Error al guardar los cambios en la base de datos", ex);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al guardar cambios: {ex.Message}", ex);
                throw new RepositoryException("Error al guardar los cambios", ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas de incidencias mediante un procedimiento almacenado
        /// </summary>
        public List<EstadisticaIncidencias> ObtenerEstadisticas()
        {
            try
            {
                _logger.Info("Obteniendo estadísticas de incidencias");
                var estadisticas = new List<EstadisticaIncidencias>();

                using (var connection = _context.Database.Connection)
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ObtenerEstadisticasIncidencias";
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                estadisticas.Add(new EstadisticaIncidencias
                                {
                                    Categoria = reader.GetString(0),
                                    Cantidad = reader.GetInt32(1)
                                });
                            }
                        }
                    }
                }

                return estadisticas;
            }
            catch (SqlException ex)
            {
                _logger.Error($"Error de SQL al obtener estadísticas: {ex.Message}", ex);
                throw new RepositoryException("Error al obtener las estadísticas de la base de datos", ex);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener estadísticas: {ex.Message}", ex);
                throw new RepositoryException("Error al obtener las estadísticas", ex);
            }
        }
    }

    /// <summary>
    /// Excepción personalizada para errores generales del repositorio
    /// </summary>
    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException = null) 
            : base(message, innerException) { }
    }

    /// <summary>
    /// Excepción personalizada para cuando no se encuentra una incidencia
    /// </summary>
    public class IncidenciaNotFoundException : Exception
    {
        public IncidenciaNotFoundException(string message) 
            : base(message) { }
    }

    /// <summary>
    /// Excepción personalizada para errores de concurrencia
    /// </summary>
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message, Exception innerException = null) 
            : base(message, innerException) { }
    }
}
