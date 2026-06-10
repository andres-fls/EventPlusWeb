using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    public class HomeController : Controller
    {
        private EventoService eventoService = new EventoService();
        private UsuarioService usuarioService = new UsuarioService();
        private CategoriaService categoriaService = new CategoriaService();
        private InscripcionService inscripcionService = new InscripcionService();
        private AprendizService aprendizService = new AprendizService();

        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            string rol = Session["UsuarioRol"] != null ? Session["UsuarioRol"].ToString() : "";

            if (rol == "Admin")
            {
                return DashboardAdmin();
            }
            else
            {
                return DashboardAprendiz();
            }
        }

        private ActionResult DashboardAdmin()
        {
            // Totales
            var eventos = eventoService.ObtenerTodos();
            var usuarios = usuarioService.ObtenerTodos();
            var categorias = categoriaService.ObtenerTodas();
            var inscripciones = inscripcionService.ObtenerTodas();

            ViewBag.TotalEventos = eventos.Count;
            ViewBag.TotalUsuarios = usuarios.Count;
            ViewBag.TotalCategorias = categorias.Count;
            ViewBag.TotalInscripciones = inscripciones.Count;
            ViewBag.EventosActivos = eventos.Count(e => e.EstadoEvento == "Activo");
            ViewBag.EventosInactivos = eventos.Count(e => e.EstadoEvento != "Activo");
            ViewBag.InscripcionesActivas = inscripciones.Count(i => i.EstadoInscripcion == "Activa");
            ViewBag.InscripcionesCanceladas = inscripciones.Count(i => i.EstadoInscripcion == "Cancelada");

            // Eventos por categoría (para gráfica de torta)
            var eventosPorCategoria = eventos.GroupBy(e => e.NombreCategoria)
                .Select(g => new { Categoria = g.Key, Cantidad = g.Count() })
                .ToList();
            ViewBag.CategoriasLabels = string.Join(",", eventosPorCategoria.Select(x => "'" + x.Categoria + "'"));
            ViewBag.CategoriasData = string.Join(",", eventosPorCategoria.Select(x => x.Cantidad.ToString()));

            // Inscripciones por mes (últimos 6 meses - para gráfica de líneas)
            var mesesLabels = new List<string>();
            var mesesData = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                mesesLabels.Add(fecha.ToString("MMM yyyy"));
                int count = inscripciones.Count(ins => ins.FechaInscripcion.Month == fecha.Month && ins.FechaInscripcion.Year == fecha.Year);
                mesesData.Add(count);
            }
            ViewBag.MesesLabels = string.Join(",", mesesLabels.Select(x => "'" + x + "'"));
            ViewBag.MesesData = string.Join(",", mesesData);

            // Últimos 5 eventos
            ViewBag.UltimosEventos = eventos.Take(5).ToList();

            ViewBag.EsDashboardAdmin = true;
            return View("Index");
        }

        private ActionResult DashboardAprendiz()
        {
            int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
            var aprendiz = aprendizService.ObtenerPorUsuarioId(usuarioId);

            if (aprendiz == null)
            {
                ViewBag.TienePerfilAprendiz = false;
                ViewBag.EsDashboardAdmin = false;
                return View("Index");
            }

            ViewBag.TienePerfilAprendiz = true;

            // Mis inscripciones
            var misInscripciones = inscripcionService.ObtenerPorAprendiz(aprendiz.IdAprendiz);
            ViewBag.MisInscripcionesActivas = misInscripciones.Count(i => i.EstadoInscripcion == "Activa");
            ViewBag.MisInscripcionesCanceladas = misInscripciones.Count(i => i.EstadoInscripcion == "Cancelada");
            ViewBag.TotalMisInscripciones = misInscripciones.Count;

            // Próximos eventos disponibles
            var eventosActivos = eventoService.ObtenerActivos();
            ViewBag.EventosDisponibles = eventosActivos.Count;
            ViewBag.ProximosEventos = eventosActivos.Take(5).ToList();

            // Eventos por categoría (para gráfica)
            var categorias = categoriaService.ObtenerTodas();
            var eventosTodos = eventoService.ObtenerActivos();
            var eventosPorCategoria = eventosTodos.GroupBy(e => e.NombreCategoria)
                .Select(g => new { Categoria = g.Key, Cantidad = g.Count() })
                .ToList();
            ViewBag.CategoriasLabels = string.Join(",", eventosPorCategoria.Select(x => "'" + x.Categoria + "'"));
            ViewBag.CategoriasData = string.Join(",", eventosPorCategoria.Select(x => x.Cantidad.ToString()));

            // Últimas inscripciones
            ViewBag.UltimasInscripciones = misInscripciones.Take(5).ToList();

            ViewBag.EsDashboardAdmin = false;
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
