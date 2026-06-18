using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Models;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class GruposController : Controller
    {
        private GrupoService grupoService = new GrupoService();
        private EventoService eventoService = new EventoService();
        private InscripcionService inscripcionService = new InscripcionService();
        private AprendizService aprendizService = new AprendizService();

        // GET: Grupos (Admin ve todos)
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Grupo> grupos = grupoService.ObtenerTodos();
            return View(grupos);
        }

        // GET: Grupos/PorEvento/5
        public ActionResult PorEvento(int id)
        {
            Evento evento = eventoService.ObtenerPorId(id);

            if (evento == null)
            {
                return HttpNotFound();
            }

            List<Grupo> grupos = grupoService.ObtenerPorEvento(id);
            ViewBag.Evento = evento;
            return View(grupos);
        }

        // GET: Grupos/Crear?eventoId=5
        [HttpGet]
        public ActionResult Crear(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);

            if (evento == null)
            {
                return HttpNotFound();
            }

            ViewBag.Evento = evento;
            Grupo grupo = new Grupo();
            grupo.Evento_idEvento = eventoId;
            return View(grupo);
        }

        // POST: Grupos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Grupo grupo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Evento = eventoService.ObtenerPorId(grupo.Evento_idEvento);
                return View(grupo);
            }

            bool creado = grupoService.Crear(grupo);

            if (creado)
            {
                TempData["Mensaje"] = "Grupo creado exitosamente.";
                return RedirectToAction("PorEvento", new { id = grupo.Evento_idEvento });
            }

            ViewBag.Error = "Error al crear el grupo.";
            ViewBag.Evento = eventoService.ObtenerPorId(grupo.Evento_idEvento);
            return View(grupo);
        }

        // GET: Grupos/Editar/5
        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Grupo grupo = grupoService.ObtenerPorId(id);

            if (grupo == null)
            {
                return HttpNotFound();
            }

            return View(grupo);
        }

        // POST: Grupos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Grupo grupo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                return View(grupo);
            }

            bool actualizado = grupoService.Actualizar(grupo);

            if (actualizado)
            {
                TempData["Mensaje"] = "Grupo actualizado exitosamente.";
                return RedirectToAction("PorEvento", new { id = grupo.Evento_idEvento });
            }

            ViewBag.Error = "Error al actualizar el grupo.";
            return View(grupo);
        }

        // POST: Grupos/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            bool eliminado = grupoService.Eliminar(id);

            if (eliminado)
            {
                TempData["Mensaje"] = "Grupo eliminado exitosamente.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el grupo.";
            }

            return RedirectToAction("PorEvento", new { id = eventoId });
        }

        // GET: Grupos/CrearGrupo?eventoId=5 (para Usuario)
        [HttpGet]
        public ActionResult CrearGrupo(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);

            if (evento == null)
            {
                return HttpNotFound();
            }

            if (evento.TipoEvento != "Grupal")
            {
                TempData["Error"] = "Este evento no es de tipo grupal.";
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de crear un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            if (inscripcionService.YaInscritoEnEvento(aprendiz.IdAprendiz, eventoId))
            {
                TempData["Error"] = "Ya perteneces a un grupo de este evento.";
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }

            ViewBag.Evento = evento;
            return View(new Grupo { Evento_idEvento = eventoId });
        }

        // POST: Grupos/CrearGrupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearGrupo(int eventoId, string nombreGrupo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (string.IsNullOrEmpty(nombreGrupo))
            {
                TempData["Error"] = "El nombre del grupo no puede estar vacío.";
                return RedirectToAction("CrearGrupo", new { eventoId = eventoId });
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de crear un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);
            if (evento == null)
            {
                return HttpNotFound();
            }

            int maxIntegrantes = 0;
            if (evento.MaxIntegrantesGrupo.HasValue)
            {
                maxIntegrantes = evento.MaxIntegrantesGrupo.Value;
            }

            string codigo = grupoService.CrearGrupoConLider(eventoId, nombreGrupo, aprendiz.IdAprendiz, maxIntegrantes);

            if (codigo != null)
            {
                TempData["Mensaje"] = "Grupo creado. Comparte este código con tu equipo: " + codigo;
                TempData["CodigoGrupo"] = codigo;
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }
            else
            {
                TempData["Error"] = string.IsNullOrEmpty(grupoService.UltimoError) ? "Error al crear el grupo." : grupoService.UltimoError;
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }
        }

        // GET: Grupos/UnirseGrupo?eventoId=5 (para Usuario)
        [HttpGet]
        public ActionResult UnirseGrupo(int eventoId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Evento evento = eventoService.ObtenerPorId(eventoId);

            if (evento == null)
            {
                return HttpNotFound();
            }

            if (evento.TipoEvento != "Grupal")
            {
                TempData["Error"] = "Este evento no es de tipo grupal.";
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de unirte a un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            if (inscripcionService.YaInscritoEnEvento(aprendiz.IdAprendiz, eventoId))
            {
                TempData["Error"] = "Ya perteneces a un grupo de este evento.";
                return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
            }

            ViewBag.Evento = evento;
            return View();
        }

        // POST: Grupos/UnirseGrupo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnirseGrupo(int eventoId, string codigo)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Usuario")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (string.IsNullOrEmpty(codigo))
            {
                TempData["Error"] = "El código de grupo no puede estar vacío.";
                return RedirectToAction("UnirseGrupo", new { eventoId = eventoId });
            }

            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                TempData["Error"] = "Debes completar tu perfil de aprendiz antes de unirte a un grupo.";
                return RedirectToAction("Crear", "Aprendiz");
            }

            ResultadoInscripcion r = inscripcionService.InscribirGrupal(aprendiz.IdAprendiz, eventoId, codigo);

            if (r.Exito)
            {
                TempData["Mensaje"] = r.Mensaje;
            }
            else
            {
                TempData["Error"] = r.Mensaje;
            }

            return RedirectToAction("Detalle", "Eventos", new { id = eventoId });
        }
    }
}