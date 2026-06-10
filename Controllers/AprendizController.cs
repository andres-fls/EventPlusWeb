using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class AprendizController : Controller
    {
        private AprendizService aprendizService = new AprendizService();
        private FichaService fichaService = new FichaService();

        // GET: Aprendiz (Admin ve listado de todos)
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Aprendiz> aprendices = aprendizService.ObtenerTodos();
            return View(aprendices);
        }

        // GET: Aprendiz/MiPerfil (Usuario ve su propio perfil de aprendiz)
        public ActionResult MiPerfil()
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                return RedirectToAction("Crear");
            }

            return View(aprendiz);
        }

        // GET: Aprendiz/Crear (Usuario crea su perfil de aprendiz)
        [HttpGet]
        public ActionResult Crear()
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            // Verificar si ya tiene perfil
            if (aprendizService.ExistePerfilAprendiz(usuarioId))
            {
                TempData["Error"] = "Ya tienes un perfil de aprendiz.";
                return RedirectToAction("MiPerfil");
            }

            ViewBag.Fichas = fichaService.ObtenerActivas();
            return View();
        }

        // POST: Aprendiz/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Aprendiz aprendiz)
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

            // Verificar si ya tiene perfil
            if (aprendizService.ExistePerfilAprendiz(usuarioId))
            {
                TempData["Error"] = "Ya tienes un perfil de aprendiz.";
                return RedirectToAction("MiPerfil");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Fichas = fichaService.ObtenerActivas();
                return View(aprendiz);
            }

            aprendiz.Usuario_idUsuario = usuarioId;

            bool creado = aprendizService.Crear(aprendiz);

            if (creado)
            {
                TempData["Mensaje"] = "Perfil de aprendiz creado exitosamente.";
                return RedirectToAction("MiPerfil");
            }

            ViewBag.Error = "Error al crear el perfil. La cédula puede estar duplicada.";
            ViewBag.Fichas = fichaService.ObtenerActivas();
            return View(aprendiz);
        }

        // GET: Aprendiz/Editar
        [HttpGet]
        public ActionResult Editar()
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                return RedirectToAction("Crear");
            }

            ViewBag.Fichas = fichaService.ObtenerActivas();
            return View(aprendiz);
        }

        // POST: Aprendiz/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Aprendiz aprendiz)
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendizActual = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendizActual == null)
            {
                return RedirectToAction("Crear");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Fichas = fichaService.ObtenerActivas();
                return View(aprendiz);
            }

            aprendiz.IdAprendiz = aprendizActual.IdAprendiz;
            aprendiz.Usuario_idUsuario = usuarioId;

            bool actualizado = aprendizService.Actualizar(aprendiz);

            if (actualizado)
            {
                TempData["Mensaje"] = "Perfil actualizado exitosamente.";
                return RedirectToAction("MiPerfil");
            }

            ViewBag.Error = "Error al actualizar el perfil.";
            ViewBag.Fichas = fichaService.ObtenerActivas();
            return View(aprendiz);
        }
    }
}