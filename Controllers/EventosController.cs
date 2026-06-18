using EventPlusWeb1.Filters;
using EventPlusWeb1.Helpers;
using EventPlusWeb1.Models;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

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
        public ActionResult Index(int? mes)
        {
            List<Evento> eventos;

            if (Session["UsuarioRol"] != null && Session["UsuarioRol"].ToString() == "Admin")
            {
                eventos = eventoService.ObtenerTodos();
            }
            else
            {
                eventos = eventoService.ObtenerActivos();
            }

            if (mes.HasValue && mes.Value > 0)
            {
                eventos = eventos.FindAll(e => e.FechaInicioEvento.Month == mes.Value);
                ViewBag.MesSeleccionado = mes.Value;
            }
            else
            {
                ViewBag.MesSeleccionado = 0;
            }

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

            ViewBag.Inscripciones = inscripcionService.ObtenerPorEvento(id);

            if (evento.TipoEvento == "Grupal")
            {
                ViewBag.Grupos = grupoService.ObtenerGruposPorEvento(id);
            }

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

            if (evento.TipoEvento != "Grupal")
            {
                evento.MaxIntegrantesGrupo = null;
            }
            else if (evento.TipoEvento == "Grupal" && (!evento.MaxIntegrantesGrupo.HasValue || evento.MaxIntegrantesGrupo.Value <= 0))
            {
                ModelState.AddModelError("MaxIntegrantesGrupo", "Debes indicar el número de integrantes por grupo.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(categoriaService.ObtenerTodas(), "IdCategoria", "NombreCategoria");
                return View(evento);
            }

            DateTime ahora = DateTimeHelper.AhoraEnColombia();

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

            if (evento.TipoEvento != "Grupal")
            {
                evento.MaxIntegrantesGrupo = null;
            }
            else if (evento.TipoEvento == "Grupal" && (!evento.MaxIntegrantesGrupo.HasValue || evento.MaxIntegrantesGrupo.Value <= 0))
            {
                ModelState.AddModelError("MaxIntegrantesGrupo", "Debes indicar el número de integrantes por grupo.");
            }

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

        // POST: Eventos/Eliminar
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

        // POST: Eventos/Inscribirse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribirse(int eventoId, int? grupoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);

            if (evento == null)
            {
                return HttpNotFound();
            }

            if (string.Equals(evento.TipoEvento, "Grupal", StringComparison.OrdinalIgnoreCase) && !grupoId.HasValue)
            {
                TempData["Error"] = "Este evento es grupal. Debes crear un grupo o unerte a uno existente desde el detalle.";
                return RedirectToAction("Detalle", new { id = eventoId });
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de inscribirte.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            if (inscripcionService.YaEstaInscrito(aprendiz.IdAprendiz, eventoId))
            {
                TempData["Error"] = "Ya estás inscrito en este evento.";
                return RedirectToAction("Detalle", new { id = eventoId });
            }

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

        // GET: Eventos/CrearGrupo
        [HttpGet]
        public ActionResult CrearGrupo(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
                return RedirectToAction("Index");

            Evento evento = eventoService.ObtenerPorId(eventoId);
            if (evento == null || evento.TipoEvento != "Grupal")
                return RedirectToAction("Index");

            ViewBag.Evento = evento;
            return View(evento);
        }

        // POST: Eventos/CrearGrupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearGrupo(int eventoId, string nombreGrupo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
                return RedirectToAction("Index");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de crear un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);
            if (evento == null || evento.TipoEvento != "Grupal")
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(nombreGrupo))
            {
                ViewBag.Error = "El nombre del grupo es obligatorio.";
                ViewBag.Evento = evento;
                return View(evento);
            }

            string codigo = grupoService.CrearGrupoConLider(eventoId, nombreGrupo.Trim(), aprendiz.IdAprendiz, evento.MaxIntegrantesGrupo.Value);

            if (codigo == null)
            {
                ViewBag.Error = grupoService.UltimoError ?? "No se pudo crear el grupo.";
                ViewBag.Evento = evento;
                return View(evento);
            }

            TempData["CodigoGrupo"] = codigo;
            TempData["NombreGrupoCreado"] = nombreGrupo.Trim();
            return RedirectToAction("Detalle", new { id = eventoId });
        }

        // GET: Eventos/UnirseGrupo
        [HttpGet]
        public ActionResult UnirseGrupo(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
                return RedirectToAction("Index");

            Evento evento = eventoService.ObtenerPorId(eventoId);
            if (evento == null || evento.TipoEvento != "Grupal")
                return RedirectToAction("Index");

            ViewBag.Evento = evento;
            return View(evento);
        }

        // POST: Eventos/UnirseGrupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnirseGrupo(int eventoId, string codigo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
                return RedirectToAction("Index");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de unirte a un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);
            if (evento == null || evento.TipoEvento != "Grupal")
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(codigo))
            {
                ViewBag.Error = "Debes ingresar un código de grupo.";
                ViewBag.Evento = evento;
                return View(evento);
            }

            ResultadoInscripcion resultado = inscripcionService.InscribirGrupal(aprendiz.IdAprendiz, eventoId, codigo.Trim().ToUpper());

            if (!resultado.Exito)
            {
                ViewBag.Error = resultado.Mensaje;
                ViewBag.Evento = evento;
                return View(evento);
            }

            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction("Detalle", new { id = eventoId });
        }

        // POST: Eventos/CancelarInscripcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarInscripcion(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
                return RedirectToAction("Index");

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "No se encontró el perfil del aprendiz.";
                return RedirectToAction("Index");
            }

            // 1. Traemos la lista como una colección dinámica para saltarnos la restricción del compilador
            System.Collections.IEnumerable inscripcionesDelEvento = inscripcionService.ObtenerPorEvento(eventoId);
            int inscripcionIdEncontrada = 0;

            if (inscripcionesDelEvento != null)
            {
                // 2. Recorremos con dynamic para evaluar las propiedades en caliente
                foreach (dynamic ins in inscripcionesDelEvento)
                {
                    try
                    {
                        // Probamos con ambos nombres posibles (con o sin prefijo) de forma segura
                        if (ins.Aprendiz_idAprendiz == aprendiz.IdAprendiz || ins.IdAprendiz == aprendiz.IdAprendiz)
                        {
                            inscripcionIdEncontrada = ins.IdInscripcion;
                            break;
                        }
                    }
                    catch
                    {
                        // Si falla la primera propiedad por nombre, intentamos con minúscula por si acaso
                        try
                        {
                            if (ins.idAprendiz == aprendiz.IdAprendiz)
                            {
                                inscripcionIdEncontrada = ins.idInscripcion;
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }

            // 3. Ejecutamos la cancelación con el ID que encontramos
            if (inscripcionIdEncontrada > 0)
            {
                inscripcionService.Cancelar(inscripcionIdEncontrada);
                TempData["Mensaje"] = "Inscripción cancelada exitosamente. Tu cupo ha sido liberado.";
            }
            else
            {
                TempData["Error"] = "No se pudo procesar la cancelación de la inscripción de forma automática.";
            }

            return RedirectToAction("Detalle", new { id = eventoId });
        }
    }
}