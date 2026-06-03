using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;
using System.Web.Mvc;

namespace EventPlusWeb1.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController()
        {
            _usuarioService = new UsuarioService();
        }

        // GET: Usuarios/Login
        public ActionResult Login()
        {
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Eventos");
            }
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string correo, string contrasena)
        {
            Usuario usuario = _usuarioService.Login(correo, contrasena);
            if (usuario != null)
            {
                Session["UsuarioId"] = usuario.Id;
                Session["UsuarioNombre"] = usuario.Nombre;
                Session["UsuarioRol"] = usuario.Rol;
                return RedirectToAction("Index", "Eventos");
            }
            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }

        // GET: Usuarios/Registro
        public ActionResult Registro()
        {
            return View();
        }

        // POST: Usuarios/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Correo) || string.IsNullOrEmpty(usuario.Contrasena))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View();
            }
            bool resultado = _usuarioService.Registrar(usuario);
            if (resultado)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Error = "El correo ya está registrado o hubo un error al registrar";
            return View();
        }

        // GET: Usuarios/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Login");
            }
            var usuarios = _usuarioService.ObtenerTodos();
            return View(usuarios);
        }
    }
}