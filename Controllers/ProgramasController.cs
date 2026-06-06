using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class ProgramasController : Controller
    {
        private ProgramaService programaService = new ProgramaService();

        // GET: Programas
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Programa> programas = programaService.ObtenerTodos();
            return View(programas);
        }

        // GET: Programas/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            return View();
        }

        // POST: Programas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Programa programa)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                return View(programa);
            }

            bool creado = programaService.Crear(programa);

            if (creado)
            {
                TempData["Mensaje"] = "Programa creado exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al crear el programa.";
            return View(programa);
        }

        // GET: Programas/Editar/5
        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Programa programa = programaService.ObtenerPorId(id);

            if (programa == null)
            {
                return HttpNotFound();
            }

            return View(programa);
        }

        // POST: Programas/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Programa programa)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                return View(programa);
            }

            bool actualizado = programaService.Actualizar(programa);

            if (actualizado)
            {
                TempData["Mensaje"] = "Programa actualizado exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al actualizar el programa.";
            return View(programa);
        }

        // POST: Programas/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            bool eliminado = programaService.Eliminar(id);

            if (eliminado)
            {
                TempData["Mensaje"] = "Programa eliminado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar el programa. Puede tener fichas asociadas.";
            }

            return RedirectToAction("Index");
        }
    }
}