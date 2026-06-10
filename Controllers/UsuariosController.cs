using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EventPlusWeb1.Filters;
using EventPlusWeb1.Models.Entities;
using EventPlusWeb1.Services;

namespace EventPlusWeb1.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuarioService usuarioService = new UsuarioService();
        private AprendizService aprendizService = new AprendizService();
        private FichaService fichaService = new FichaService();
        private ProgramaService programaService = new ProgramaService();

        // GET: Usuarios/Login
        [HttpGet]
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
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Debe ingresar correo y contraseña.";
                return View();
            }

            Usuario usuario = usuarioService.Login(correo, contrasena);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos, o la cuenta está inactiva.";
                return View();
            }

            // Crear sesión
            Session["UsuarioId"] = usuario.IdUsuario;
            Session["UsuarioNombre"] = usuario.Nombre;
            Session["UsuarioRol"] = usuario.Rol;
            Session["UsuarioCorreo"] = usuario.Correo;

            return RedirectToAction("Index", "Home");
        }

        // GET: Usuarios/Registro
        [HttpGet]
        public ActionResult Registro()
        {
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Eventos");
            }

            ViewBag.Fichas = fichaService.ObtenerActivas();
            ViewBag.Programas = programaService.ObtenerTodos();

            return View();
        }

        // POST: Usuarios/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(string Nombre, string Correo, string Contrasena, string ConfirmarContrasena,
            string Cedula, string Telefono, int? Edad, string Genero, int Ficha_idFicha)
        {
            // Cargar datos para dropdowns en caso de error
            ViewBag.Fichas = fichaService.ObtenerActivas();
            ViewBag.Programas = programaService.ObtenerTodos();

            if (string.IsNullOrWhiteSpace(Contrasena))
            {
                ViewBag.Error = "La contraseña es obligatoria.";
                return View();
            }

            if (Contrasena != ConfirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            if (Contrasena.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Cedula))
            {
                ViewBag.Error = "La cédula es obligatoria.";
                return View();
            }

            // Crear usuario
            Usuario usuario = new Usuario();
            usuario.Nombre = Nombre;
            usuario.Correo = Correo;
            usuario.Rol = "Usuario";

            bool registrado = usuarioService.Registrar(usuario, Contrasena);

            if (!registrado)
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View();
            }

            // Obtener el usuario recién creado para tener su ID
            Usuario usuarioCreado = usuarioService.ObtenerPorCorreo(Correo);

            // Crear aprendiz
            Aprendiz aprendiz = new Aprendiz();
            aprendiz.Usuario_idUsuario = usuarioCreado.IdUsuario;
            aprendiz.Ficha_idFicha = Ficha_idFicha;
            aprendiz.Cedula = Cedula;
            aprendiz.Telefono = Telefono;
            aprendiz.Edad = Edad;
            aprendiz.Genero = Genero;

            aprendizService.Crear(aprendiz);

            TempData["Mensaje"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // GET: Usuarios/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Usuarios/Index (solo Admin - listado de usuarios)
        [AuthFilter]
        public ActionResult Index()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            List<Usuario> usuarios = usuarioService.ObtenerTodos();
            return View(usuarios);
        }

        // POST: Usuarios/CambiarEstado (solo Admin)
        [HttpPost]
        [AuthFilter]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, bool estado)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Eventos");
            }

            usuarioService.CambiarEstado(id, estado);
            TempData["Mensaje"] = "Estado del usuario actualizado.";
            return RedirectToAction("Index");
        }
    }
}