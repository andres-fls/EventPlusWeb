using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class CategoriasController : Controller
    {
        private CategoriaService categoriaService = new CategoriaService();
        private EventoService eventoService = new EventoService();

        // GET: Categorias
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Categoria> categorias = categoriaService.ObtenerTodas();
            return View(categorias);
        }

        // GET: Categorias/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            return View();
        }

        // POST: Categorias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Categoria categoria)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            bool creado = categoriaService.Crear(categoria);

            if (creado)
            {
                TempData["Mensaje"] = "Categoría creada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al crear la categoría. Puede que ya exista.";
            return View(categoria);
        }

        // GET: Categorias/Editar/5
        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            Categoria categoria = categoriaService.ObtenerPorId(id);

            if (categoria == null)
            {
                return HttpNotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Categoria categoria)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            bool actualizado = categoriaService.Actualizar(categoria);

            if (actualizado)
            {
                TempData["Mensaje"] = "Categoría actualizada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al actualizar la categoría.";
            return View(categoria);
        }

        // POST: Categorias/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            bool eliminado = categoriaService.Eliminar(id);

            if (eliminado)
            {
                TempData["Mensaje"] = "Categoría eliminada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar la categoría. Puede tener eventos asociados.";
            }

            return RedirectToAction("Index");
        }
        public ActionResult EventosPorCategoria(int id)
        {
            var categoria = categoriaService.ObtenerPorId(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }

            ViewBag.NombreCategoria = categoria.NombreCategoria;
            var eventos = eventoService.ObtenerPorCategoria(id);
            return View(eventos);
        }
    }
}