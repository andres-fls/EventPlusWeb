using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;
using FastReport;
using FastReport.Export.PdfSimple;

namespace EventPlusWeb1.Controllers
{
    public class ReportesController : Controller
    {
        private readonly EventoService _eventoService;
        private readonly InscripcionService _inscripcionService;
        private readonly UsuarioService _usuarioService;

        public ReportesController()
        {
            _eventoService = new EventoService();
            _inscripcionService = new InscripcionService();
            _usuarioService = new UsuarioService();
        }

        // GET: Reportes
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            return View();
        }

        // GET: Reportes/EventosPDF
        public ActionResult EventosPDF()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var eventos = _eventoService.ObtenerTodos();

            using (Report report = new Report())
            {
                string reportPath = Server.MapPath("~/Reports/Eventos.frx");
                report.Load(reportPath);

                report.RegisterData(eventos, "Eventos");
                report.Prepare();

                using (PDFSimpleExport pdfExport = new PDFSimpleExport())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        report.Export(pdfExport, ms);
                        ms.Position = 0;
                        return File(ms.ToArray(), "application/pdf", "Reporte_Eventos_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                    }
                }
            }
        }

        // GET: Reportes/InscripcionesPDF
        public ActionResult InscripcionesPDF()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var eventos = _eventoService.ObtenerTodos();
            var inscripciones = new List<object>();

            foreach (var evento in eventos)
            {
                var inscritos = _inscripcionService.ObtenerPorEvento(evento.IdEvento);
                foreach (var ins in inscritos)
                {
                    inscripciones.Add(new
                    {
                        EventoTitulo = evento.NombreEvento,
                        UsuarioNombre = ins.NombreAprendiz,
                        FechaInscripcion = ins.FechaInscripcion
                    });
                }
            }

            using (Report report = new Report())
            {
                string reportPath = Server.MapPath("~/Reports/Inscripciones.frx");
                report.Load(reportPath);

                report.RegisterData(inscripciones, "Inscripciones");
                report.Prepare();

                using (PDFSimpleExport pdfExport = new PDFSimpleExport())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        report.Export(pdfExport, ms);
                        ms.Position = 0;
                        return File(ms.ToArray(), "application/pdf", "Reporte_Inscripciones_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                    }
                }
            }
        }
    }
}