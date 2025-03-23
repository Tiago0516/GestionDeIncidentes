// Importación de espacios de nombres necesarios
using SistemaIncidencias.Models;
using SistemaIncidencias.Repositories;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using log4net;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace SistemaIncidencias.Controllers
{
    // Controlador que maneja las operaciones relacionadas con incidencias
    public class IncidenciasController : Controller
    {
        // Repositorio para acceder a los datos de incidencias
        private readonly IIncidenciaRepository _incidenciaRepository;
        // Logger para registrar eventos y errores
        private static readonly ILog _logger = LogManager.GetLogger(typeof(IncidenciasController));

        // Constructor que recibe el repositorio por inyección de dependencias
        public IncidenciasController(IIncidenciaRepository incidenciaRepository)
        {
            _incidenciaRepository = incidenciaRepository;
        }

        // GET: Muestra el listado de incidencias con filtros y paginación
        public ActionResult Index(string estado, string prioridad, string tecnico, int? page)
        {
            return RedirectToAction("Reporte");
        }

        // GET: Muestra los detalles de una incidencia específica
        public ActionResult Details(int id)
        {
            try
            {
                _logger.Info($"Consultando detalles de la incidencia {id}");
                var incidencia = _incidenciaRepository.ObtenerPorId(id);
                return View(incidencia);
            }
            catch (IncidenciaNotFoundException ex)
            {
                _logger.Warn($"Intento de acceder a una incidencia inexistente: {id}");
                return HttpNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener detalles de la incidencia {id}: {ex.Message}", ex);
                TempData["Error"] = "Ocurrió un error al cargar los detalles de la incidencia.";
                return RedirectToAction("Reporte");
            }
        }

        // GET: Muestra el formulario para crear una nueva incidencia
        public ActionResult Create()
        {
            try
            {
                _logger.Info("Accediendo al formulario de creación de incidencia");
                using (var context = new SistemaIncidenciasContext())
                {
                    ViewBag.Usuarios = context.Usuarios.ToList();
                    ViewBag.Tecnicos = context.Tecnicos.ToList();
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al cargar el formulario de creación: {ex.Message}", ex);
                TempData["Error"] = "Error al cargar el formulario de creación.";
                return RedirectToAction("Reporte");
            }
        }

        // POST: Procesa la creación de una nueva incidencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Incidencia incidencia, string comentarioInicial)
        {
            try
            {
                // Valida el modelo
                if (!ModelState.IsValid)
                {
                    _logger.Warn("Intento de crear incidencia con modelo inválido");
                    using (var context = new SistemaIncidenciasContext())
                    {
                        ViewBag.Usuarios = context.Usuarios.ToList();
                        ViewBag.Tecnicos = context.Tecnicos.ToList();
                    }
                    return View(incidencia);
                }

                _logger.Info($"Creando nueva incidencia: {incidencia.Titulo}");
                incidencia.FechaCreacion = DateTime.Now;
                incidencia.FechaActualizacion = DateTime.Now;

                // Agrega comentario inicial si existe
                if (!string.IsNullOrWhiteSpace(comentarioInicial))
                {
                    _logger.Debug($"Agregando comentario inicial a la incidencia: {comentarioInicial}");
                    incidencia.Comentarios = new List<Comentario>
                    {
                        new Comentario
                        {
                            Texto = comentarioInicial,
                            Fecha = DateTime.Now
                        }
                    };
                }

                _incidenciaRepository.Agregar(incidencia);
                _incidenciaRepository.Guardar();

                _logger.Info($"Incidencia creada exitosamente con ID: {incidencia.Id}");
                TempData["Success"] = "Incidencia creada exitosamente.";
                return RedirectToAction("Reporte");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al crear la incidencia: {ex.Message}", ex);
                ModelState.AddModelError("", "Ocurrió un error al crear la incidencia.");
                
                using (var context = new SistemaIncidenciasContext())
                {
                    ViewBag.Usuarios = context.Usuarios.ToList();
                    ViewBag.Tecnicos = context.Tecnicos.ToList();
                }
                return View(incidencia);
            }
        }

        // GET: Muestra el formulario para editar una incidencia
        public ActionResult Edit(int id)
        {
            try
            {
                _logger.Info($"Accediendo al formulario de edición de incidencia {id}");
                var incidencia = _incidenciaRepository.ObtenerPorId(id);
                if (incidencia == null)
                {
                    return RedirectToAction("Reporte");
                }

                // Establece valores por defecto para Estado y Prioridad
                if (string.IsNullOrEmpty(incidencia.Estado))
                {
                    incidencia.Estado = "Abierto";
                }
                if (string.IsNullOrEmpty(incidencia.Prioridad))
                {
                    incidencia.Prioridad = "Media";
                }

                // Inicializa las listas de estados y prioridades
                ViewBag.Estados = new List<string> { "Abierto", "En Proceso", "Resuelto", "Cerrado" };
                ViewBag.Prioridades = new List<string> { "Baja", "Media", "Alta", "Crítica" };
                
                return View(incidencia);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al cargar el formulario de edición: {ex.Message}", ex);
                return RedirectToAction("Reporte");
            }
        }

        // POST: Procesa la edición de una incidencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Incidencia incidencia, string nuevoComentario)
        {
            try
            {
                // Valida el modelo
                if (!ModelState.IsValid)
                {
                    ViewBag.Estados = new List<string> { "Abierto", "En Proceso", "Resuelto", "Cerrado" };
                    ViewBag.Prioridades = new List<string> { "Baja", "Media", "Alta", "Crítica" };
                    return View(incidencia);
                }

                // Establece valores por defecto para Estado y Prioridad
                if (string.IsNullOrEmpty(incidencia.Estado))
                {
                    incidencia.Estado = "Abierto";
                }
                if (string.IsNullOrEmpty(incidencia.Prioridad))
                {
                    incidencia.Prioridad = "Media";
                }

                var incidenciaExistente = _incidenciaRepository.ObtenerPorId(incidencia.Id);
                if (incidenciaExistente == null)
                {
                    return HttpNotFound();
                }

                // Actualiza las propiedades
                incidenciaExistente.Estado = incidencia.Estado;
                incidenciaExistente.Prioridad = incidencia.Prioridad;
                incidenciaExistente.FechaActualizacion = DateTime.Now;

                // Agrega nuevo comentario si existe
                if (!string.IsNullOrWhiteSpace(nuevoComentario))
                {
                    incidenciaExistente.Comentarios.Add(new Comentario
                    {
                        Texto = nuevoComentario,
                        Fecha = DateTime.Now,
                        IncidenciaId = incidencia.Id
                    });
                }

                _incidenciaRepository.Actualizar(incidenciaExistente);
                _incidenciaRepository.Guardar();

                return RedirectToAction("Reporte");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al actualizar la incidencia: {ex.Message}", ex);
                ViewBag.Estados = new List<string> { "Abierto", "En Proceso", "Resuelto", "Cerrado" };
                ViewBag.Prioridades = new List<string> { "Baja", "Media", "Alta", "Crítica" };
                return View(incidencia);
            }
        }

        // POST: Elimina una incidencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _logger.Info($"Eliminando incidencia {id}");
                _incidenciaRepository.Eliminar(id);
                _incidenciaRepository.Guardar();
                _logger.Info($"Incidencia {id} eliminada exitosamente");
                return Json(new { success = true, message = "Incidencia eliminada exitosamente." });
            }
            catch (IncidenciaNotFoundException ex)
            {
                _logger.Warn($"Intento de eliminar una incidencia inexistente: {id}");
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al eliminar la incidencia {id}: {ex.Message}", ex);
                return Json(new { success = false, message = "Error al eliminar la incidencia." });
            }
        }

        // POST: Agrega un comentario a una incidencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarComentario(int id, string comentario)
        {
            try
            {
                // Valida que el comentario no esté vacío
                if (string.IsNullOrWhiteSpace(comentario))
                {
                    _logger.Warn($"Intento de agregar comentario vacío a la incidencia {id}");
                    return Json(new { success = false, message = "El comentario no puede estar vacío." });
                }

                _logger.Info($"Agregando comentario a la incidencia {id}");
                var incidencia = _incidenciaRepository.ObtenerPorId(id);
                incidencia.FechaActualizacion = DateTime.Now;
                incidencia.Comentarios.Add(new Comentario 
                { 
                    Texto = comentario, 
                    Fecha = DateTime.Now 
                });

                _incidenciaRepository.Guardar();
                _logger.Info($"Comentario agregado exitosamente a la incidencia {id}");
                return Json(new { success = true, message = "Comentario agregado exitosamente." });
            }
            catch (IncidenciaNotFoundException ex)
            {
                _logger.Warn($"Intento de agregar comentario a una incidencia inexistente: {id}");
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al agregar comentario a la incidencia {id}: {ex.Message}", ex);
                return Json(new { success = false, message = "Error al agregar el comentario." });
            }
        }

        // GET: Muestra el reporte de incidencias
        public ActionResult Reporte(int? page, string fecha, string estado, string prioridad, string tecnico)
        {
            try
            {
                _logger.Info("Accediendo al reporte de incidencias");
                int pageSize = 5;
                int pageNumber = (page ?? 1);

                using (var context = new SistemaIncidenciasContext())
                {
                    ViewBag.Tecnicos = context.Tecnicos.ToList();
                }

                var incidencias = _incidenciaRepository.ObtenerTodas()
                    .AsQueryable()
                    .Include(i => i.UsuarioReporta)
                    .Include(i => i.TecnicoAsignado);

                // Aplicar filtros
                if (!string.IsNullOrEmpty(fecha))
                {
                    try
                    {
                        var fechaFiltro = DateTime.Parse(fecha);
                        _logger.Info($"Fecha recibida para filtrar: {fechaFiltro:yyyy-MM-dd}");

                        // Crear rango de fechas para el día completo
                        var fechaInicio = fechaFiltro.Date;
                        var fechaFin = fechaInicio.AddDays(1);

                        incidencias = incidencias.Where(i => 
                            i.FechaCreacion >= fechaInicio && 
                            i.FechaCreacion < fechaFin);

                        _logger.Info($"Filtro de fecha aplicado para: {fechaFiltro:yyyy-MM-dd}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error al parsear la fecha: {fecha}, Error: {ex.Message}", ex);
                        throw;
                    }
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    incidencias = incidencias.Where(i => i.Estado == estado);
                }

                if (!string.IsNullOrEmpty(prioridad))
                {
                    incidencias = incidencias.Where(i => i.Prioridad == prioridad);
                }

                if (!string.IsNullOrEmpty(tecnico))
                {
                    var tecnicoId = int.Parse(tecnico);
                    incidencias = incidencias.Where(i => i.TecnicoAsignadoId == tecnicoId);
                }

                // Ordenar por fecha de creación descendente
                incidencias = incidencias.OrderByDescending(i => i.FechaCreacion);

                var pagedList = incidencias.ToPagedList(pageNumber, pageSize);
                
                if (pagedList == null)
                {
                    throw new Exception("Error al paginar las incidencias");
                }

                _logger.Info($"Mostrando página {pageNumber} de incidencias, {pagedList.Count()} registros");
                return View(pagedList);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error en el reporte de incidencias: {ex.Message}", ex);
                throw new Exception($"Error al cargar el reporte de incidencias: {ex.Message}", ex);
            }
        }

        // GET: Muestra el dashboard de estadísticas
        public ActionResult Dashboard()
        {
            try
            {
                _logger.Info("Accediendo al dashboard de estadísticas");
                var estadisticas = _incidenciaRepository.ObtenerEstadisticas();
                return View(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al cargar el dashboard: {ex.Message}", ex);
                TempData["Error"] = "Error al cargar las estadísticas.";
                return RedirectToAction("Reporte");
            }
        }

        // Maneja las excepciones no controladas del controlador
        protected override void OnException(ExceptionContext filterContext)
        {
            _logger.Error($"Error no manejado en {filterContext.RouteData.Values["action"]}: {filterContext.Exception.Message}", 
                filterContext.Exception);
            
            filterContext.ExceptionHandled = true;
            filterContext.Result = View("Error", new HandleErrorInfo(
                filterContext.Exception,
                filterContext.RouteData.Values["controller"].ToString(),
                filterContext.RouteData.Values["action"].ToString()));
        }
    }
}
