using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class FichasController : Controller
    {
        private FichaService fichaService = new FichaService();
        private ProgramaService programaService = new ProgramaService();

        // GET: Fichas
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Ficha> fichas = fichaService.ObtenerTodas();
            return View(fichas);
        }

        // GET: Fichas/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            ViewBag.Programas = programaService.ObtenerTodos();
            return View();
        }

        // POST: Fichas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Ficha ficha)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Programas = programaService.ObtenerTodos();
                return View(ficha);
            }

            bool creado = fichaService.Crear(ficha);

            if (creado)
            {
                TempData["Mensaje"] = "Ficha creada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al crear la ficha.";
            ViewBag.Programas = programaService.ObtenerTodos();
            return View(ficha);
        }

        // GET: Fichas/Editar/5
        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Ficha ficha = fichaService.ObtenerPorId(id);

            if (ficha == null)
            {
                return HttpNotFound();
            }

            ViewBag.Programas = programaService.ObtenerTodos();
            return View(ficha);
        }

        // POST: Fichas/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Ficha ficha)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Programas = programaService.ObtenerTodos();
                return View(ficha);
            }

            bool actualizado = fichaService.Actualizar(ficha);

            if (actualizado)
            {
                TempData["Mensaje"] = "Ficha actualizada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al actualizar la ficha.";
            ViewBag.Programas = programaService.ObtenerTodos();
            return View(ficha);
        }

        // POST: Fichas/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            bool eliminado = fichaService.Eliminar(id);

            if (eliminado)
            {
                TempData["Mensaje"] = "Ficha eliminada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar la ficha. Puede tener aprendices asociados.";
            }

            return RedirectToAction("Index");
        }
    }
}