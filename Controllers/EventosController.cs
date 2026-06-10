using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class EventosController : Controller
    {
        private EventoService eventoService = new EventoService();
        private CategoriaService categoriaService = new CategoriaService();
        private InscripcionService inscripcionService = new InscripcionService();
        private AprendizService aprendizService = new AprendizService();
        private GrupoService grupoService = new GrupoService();

        // GET: Eventos
        // 1. Agregamos el parámetro opcional 'int? mes'
        public ActionResult Index(int? mes)
        {
            List<Evento> eventos;

            // Admin ve todos, Usuario ve solo activos
            if (Session["UsuarioRol"] != null && Session["UsuarioRol"].ToString() == "Admin")
            {
                eventos = eventoService.ObtenerTodos();
            }
            else
            {
                eventos = eventoService.ObtenerActivos();
            }

            // 2. Aplicamos el filtro por mes si el usuario seleccionó uno válido (mayor a 0)
            if (mes.HasValue && mes.Value > 0)
            {
                // Filtramos usando la propiedad FechaInicioEvento
                eventos = eventos.FindAll(e => e.FechaInicioEvento.Month == mes.Value);

                // Enviamos el mes de vuelta a la vista mediante el ViewBag
                ViewBag.MesSeleccionado = mes.Value;
            }
            else
            {
                // Si no se selecciona un mes o es "Todos", enviamos un 0
                ViewBag.MesSeleccionado = 0;
            }

            // Retornamos la lista (ya filtrada si aplica) a la vista
            return View(eventos);
        }

        // GET: Eventos/Detalle/5
        public ActionResult Detalle(int id)
        {
            Evento evento = eventoService.ObtenerPorId(id);

            if (evento == null)
            {
                return HttpNotFound();
            }

            // Obtener inscripciones del evento para mostrar
            ViewBag.Inscripciones = inscripcionService.ObtenerPorEvento(id);

            // Obtener grupos si es grupal
            if (evento.TipoEvento == "Grupal")
            {
                ViewBag.Grupos = grupoService.ObtenerPorEvento(id);
            }

            // Verificar si el usuario actual ya está inscrito
            if (Session["UsuarioRol"] != null && Session["UsuarioRol"].ToString() == "Usuario")
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

                if (aprendiz != null)
                {
                    ViewBag.YaInscrito = inscripcionService.YaEstaInscrito(aprendiz.IdAprendiz, id);
                    ViewBag.AprendizId = aprendiz.IdAprendiz;
                    ViewBag.TienePerfilAprendiz = true;
                }
                else
                {
                    ViewBag.YaInscrito = false;
                    ViewBag.AprendizId = 0;
                    ViewBag.TienePerfilAprendiz = false;
                }
            }

            return View(evento);
        }

        // GET: Eventos/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index");

            ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
            return View();
        }

        // POST: Eventos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Evento evento)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index");

            evento.Usuario_idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            evento.EstadoEvento = "Activo";
            ModelState.Remove("Usuario_idUsuario");
            ModelState.Remove("EstadoEvento");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
                return View(evento);
            }

            // Validar fechas
            DateTime ahora = DateTime.Now;

            if (evento.FechaInicioEvento < ahora)
            {
                ModelState.AddModelError("FechaInicioEvento", "La fecha de inicio del evento no puede ser en el pasado.");
            }

            if (evento.FechaFinEvento <= evento.FechaInicioEvento)
            {
                ModelState.AddModelError("FechaFinEvento", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if (evento.FechaInicioInscripcion < ahora)
            {
                ModelState.AddModelError("FechaInicioInscripcion", "La fecha de inicio de inscripción no puede ser en el pasado.");
            }

            if (evento.FechaFinInscripcion <= evento.FechaInicioInscripcion)
            {
                ModelState.AddModelError("FechaFinInscripcion", "La fecha de fin de inscripción debe ser posterior a la fecha de inicio.");
            }

            if (evento.FechaFinInscripcion > evento.FechaInicioEvento)
            {
                ModelState.AddModelError("FechaFinInscripcion", "El período de inscripción debe cerrar antes de que inicie el evento.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
                return View(evento);
            }

            evento.Usuario_idUsuario = Convert.ToInt32(Session["UsuarioId"]);
            evento.EstadoEvento = "Activo";

            bool creado = eventoService.Crear(evento);

            if (creado)
            {
                TempData["Mensaje"] = "Evento creado exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al crear el evento.";
            ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
            return View(evento);
        }
        // GET: Eventos/Editar/5
        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index");

            Evento evento = eventoService.ObtenerPorId(id);

            if (evento == null)
                return HttpNotFound();

            ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
            return View(evento);
        }

        // POST: Eventos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Evento evento)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
                return View(evento);
            }

            bool actualizado = eventoService.Actualizar(evento);

            if (actualizado)
            {
                TempData["Mensaje"] = "Evento actualizado exitosamente.";
                return RedirectToAction("Detalle", new { id = evento.IdEvento });
            }

            ViewBag.Error = "Error al actualizar el evento.";
            ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
            return View(evento);
        }

        // POST: Eventos/Eliminar (solo Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index");
            }

            bool eliminado = eventoService.Eliminar(id);

            if (eliminado)
            {
                TempData["Mensaje"] = "Evento eliminado exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el evento.";
            }

            return RedirectToAction("Index");
        }

        // POST: Eventos/Inscribirse (solo Usuario con perfil de aprendiz)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribirse(int eventoId, int? grupoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index");
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de inscribirte.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            // Verificar si ya está inscrito
            if (inscripcionService.YaEstaInscrito(aprendiz.IdAprendiz, eventoId))
            {
                TempData["Error"] = "Ya estás inscrito en este evento.";
                return RedirectToAction("Detalle", new { id = eventoId });
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);

            if (evento == null)
            {
                return HttpNotFound();
            }

            // Verificar cupo
            int inscritos = inscripcionService.ContarInscritosPorEvento(eventoId);
            if (inscritos >= evento.CupoMaximo)
            {
                TempData["Error"] = "El evento ya alcanzó el cupo máximo.";
                return RedirectToAction("Detalle", new { id = eventoId });
            }

            bool inscrito;

            if (evento.TipoEvento == "Grupal" && grupoId.HasValue)
            {
                inscrito = inscripcionService.CrearGrupal(aprendiz.IdAprendiz, eventoId, grupoId.Value);
            }
            else
            {
                inscrito = inscripcionService.CrearIndividual(aprendiz.IdAprendiz, eventoId);
            }

            if (inscrito)
            {
                TempData["Mensaje"] = "Inscripción realizada exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al realizar la inscripción.";
            }

            return RedirectToAction("Detalle", new { id = eventoId });
        }

        // POST: Eventos/CancelarInscripcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarInscripcion(int inscripcionId, int eventoId)
        {
            inscripcionService.Cancelar(inscripcionId);
            TempData["Mensaje"] = "Inscripción cancelada.";
            return RedirectToAction("Detalle", new { id = eventoId });
        }

    }
}
