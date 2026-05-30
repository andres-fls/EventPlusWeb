using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EventPlusWeb1.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoService _eventoService;
        private readonly CategoriaService _categoriaService;
        private readonly InscripcionService _inscripcionService;

        public EventosController()
        {
            _eventoService = new EventoService();
            _categoriaService = new CategoriaService();
            _inscripcionService = new InscripcionService();
        }

        // GET: Eventos
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            var eventos = _eventoService.ObtenerTodos();
            return View(eventos);
        }

        // GET: Eventos/Crear
        public ActionResult Crear()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            ViewBag.Categorias = _categoriaService.ObtenerTodas();
            return View();
        }

        // POST: Eventos/Crear
        [HttpPost]

        public ActionResult Crear(Evento evento)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            evento.UsuarioCreadorId = (int)Session["UsuarioId"];
            bool resultado = _eventoService.Crear(evento);
            if (resultado)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Error al crear el evento";
            ViewBag.Categorias = _categoriaService.ObtenerTodas();
            return View(evento);
        }

        // GET: Eventos/Editar/5
        public ActionResult Editar(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            var evento = _eventoService.ObtenerPorId(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categorias = _categoriaService.ObtenerTodas();
            return View(evento);
        }

        // POST: Eventos/Editar
        [HttpPost]
        public ActionResult Editar(Evento evento)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            bool resultado = _eventoService.Editar(evento);
            if (resultado)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Error al editar el evento";
            ViewBag.Categorias = _categoriaService.ObtenerTodas();
            return View(evento);
        }

        // GET: Eventos/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            var evento = _eventoService.ObtenerPorId(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // POST: Eventos/EliminarConfirmado/5
        [HttpPost]
        [ActionName("Eliminar")]
        public ActionResult EliminarConfirmado(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            _eventoService.Eliminar(id);
            return RedirectToAction("Index");
        }

        // GET: Eventos/Detalle/5
        public ActionResult Detalle(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            var evento = _eventoService.ObtenerPorId(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            var inscripciones = _inscripcionService.ObtenerPorEvento(id);
            ViewBag.Inscripciones = inscripciones;
            return View(evento);
        }

        // POST: Eventos/Inscribirse/5
        [HttpPost]
        public ActionResult Inscribirse(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            int usuarioId = (int)Session["UsuarioId"];
            _inscripcionService.Inscribir(usuarioId, id);
            return RedirectToAction("Detalle", new { id = id });
        }

        // POST: Eventos/CancelarInscripcion/5
        [HttpPost]
        public ActionResult CancelarInscripcion(int id)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            int usuarioId = (int)Session["UsuarioId"];
            _inscripcionService.CancelarInscripcion(usuarioId, id);
            return RedirectToAction("Detalle", new { id = id });
        }
    }
}