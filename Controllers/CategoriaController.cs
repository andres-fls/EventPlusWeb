using System.Web.Mvc;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CategoriaService _categoriaService;

        public CategoriasController()
        {
            _categoriaService = new CategoriaService();
        }

        // GET: Categorias
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            var categorias = _categoriaService.ObtenerTodas();
            return View(categorias);
        }
    }
}
