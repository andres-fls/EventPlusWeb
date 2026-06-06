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

            return RedirectToAction("Index", "Eventos");
        }

        // GET: Usuarios/Registro
        [HttpGet]
        public ActionResult Registro()
        {
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Eventos");
            }
            return View();
        }

        // POST: Usuarios/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Usuario usuario, string contrasena, string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "La contraseña es obligatoria.";
                return View(usuario);
            }

            if (contrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View(usuario);
            }

            if (contrasena.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                return View(usuario);
            }

            // Rol por defecto para registro público
            usuario.Rol = "Usuario";

            bool registrado = usuarioService.Registrar(usuario, contrasena);

            if (!registrado)
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View(usuario);
            }

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