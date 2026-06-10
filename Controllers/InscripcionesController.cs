using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    [AuthFilter]
    public class InscripcionesController : Controller
    {
        private InscripcionService inscripcionService = new InscripcionService();
        private AprendizService aprendizService = new AprendizService();

        // GET: Inscripciones (Admin ve todas)
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("MisInscripciones");
            }

            List<Inscripcion> inscripciones = inscripcionService.ObtenerTodas();
            return View(inscripciones);
        }

        // GET: Inscripciones/MisInscripciones (Usuario ve las suyas)
        public ActionResult MisInscripciones()
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                ViewBag.SinPerfil = true;
                return View(new List<Inscripcion>());
            }

            List<Inscripcion> inscripciones = inscripcionService.ObtenerPorAprendiz(aprendiz.IdAprendiz);
            return View(inscripciones);
        }

        // POST: Inscripciones/Cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancelar(int id)
        {
            Inscripcion inscripcion = inscripcionService.ObtenerPorId(id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            // Verificar permisos: Admin puede cancelar cualquiera, Usuario solo las suyas
            if (Session["UsuarioRol"] != null && Session["UsuarioRol"].ToString() == "Usuario")
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                Aprendiz aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

                if (aprendiz == null || aprendiz.IdAprendiz != inscripcion.Aprendiz_idAprendiz)
                {
                    TempData["Error"] = "No tienes permiso para cancelar esta inscripción.";
                    return RedirectToAction("MisInscripciones");
                }
            }

            inscripcionService.Cancelar(id);
            TempData["Mensaje"] = "Inscripción cancelada exitosamente.";

            if (Session["UsuarioRol"] != null && Session["UsuarioRol"].ToString() == "Admin")
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("MisInscripciones");
        }

        // POST: Inscripciones/Eliminar (solo Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            inscripcionService.Eliminar(id);
            TempData["Mensaje"] = "Inscripción eliminada exitosamente.";
            return RedirectToAction("Index");
        }
    }
}