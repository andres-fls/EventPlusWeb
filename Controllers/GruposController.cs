using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class GruposController : Controller
    {
        private GrupoService grupoService = new GrupoService();
        private EventoService eventoService = new EventoService();

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
    }
}