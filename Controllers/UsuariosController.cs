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
                return RedirectToAction("Index", "Eventos");
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
                return RedirectToAction("Index", "Eventos");

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
            ViewBag.Fichas = fichaService.ObtenerActivas();
            ViewBag.Programas = programaService.ObtenerTodos();

            if (string.IsNullOrWhiteSpace(Nombre))
                ViewBag.ErrorNombre = "El nombre es obligatorio.";
            else if (Nombre.Trim().Length < 3)
                ViewBag.ErrorNombre = "El nombre debe tener al menos 3 caracteres.";
            else if (Nombre.Trim().Length > 100)
                ViewBag.ErrorNombre = "El nombre no puede superar los 100 caracteres.";

            if (string.IsNullOrWhiteSpace(Correo))
                ViewBag.ErrorCorreo = "El correo es obligatorio.";
            else if (!Correo.Contains("@") || !Correo.Contains("."))
                ViewBag.ErrorCorreo = "El correo no tiene un formato válido.";
            else if (Correo.Length > 100)
                ViewBag.ErrorCorreo = "El correo no puede superar los 100 caracteres.";

            if (string.IsNullOrWhiteSpace(Contrasena))
                ViewBag.ErrorContrasena = "La contraseña es obligatoria.";
            else if (Contrasena.Length < 6)
                ViewBag.ErrorContrasena = "La contraseña debe tener al menos 6 caracteres.";
            else if (Contrasena.Length > 50)
                ViewBag.ErrorContrasena = "La contraseña no puede superar los 50 caracteres.";
            else if (Contrasena != ConfirmarContrasena)
                ViewBag.ErrorContrasena = "Las contraseñas no coinciden.";

            if (string.IsNullOrWhiteSpace(Cedula))
                ViewBag.ErrorCedula = "La cédula es obligatoria.";
            else if (Cedula.Trim().Length < 6 || Cedula.Trim().Length > 15)
                ViewBag.ErrorCedula = "La cédula debe tener entre 6 y 15 caracteres.";

            if (ViewBag.ErrorNombre != null || ViewBag.ErrorCorreo != null ||
                ViewBag.ErrorContrasena != null || ViewBag.ErrorCedula != null)
                return View();

            Usuario usuario = new Usuario();
            usuario.Nombre = Nombre.Trim();
            usuario.Correo = Correo.Trim();

            bool registrado = usuarioService.Registrar(usuario, Contrasena);

            if (!registrado)
            {
                ViewBag.ErrorCorreo = "El correo ya está registrado.";
                return View();
            }

            Usuario usuarioCreado = usuarioService.ObtenerPorCorreo(Correo);

            Aprendiz aprendiz = new Aprendiz();
            aprendiz.Usuario_idUsuario = usuarioCreado.IdUsuario;
            aprendiz.Ficha_idFicha = Ficha_idFicha;
            aprendiz.Cedula = Cedula.Trim();
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

        // GET: Usuarios/Index (solo Admin)
        [AuthFilter]
        public ActionResult Index(int? programaId, int? fichaId)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index", "Eventos");

            var administradores = usuarioService.ObtenerAdministradores();
            var aprendices = usuarioService.ObtenerAprendices(programaId, fichaId);

            ViewBag.Administradores = administradores;
            ViewBag.Aprendices = aprendices;
            ViewBag.Programas = new SelectList(programaService.ObtenerTodos(), "IdPrograma", "NombrePrograma", programaId);
            ViewBag.Fichas = new SelectList(fichaService.ObtenerActivas(), "IdFicha", "CodigoFicha", fichaId);
            ViewBag.ProgramaSeleccionado = programaId;
            ViewBag.FichaSeleccionada = fichaId;

            return View();
        }

        // GET: Usuarios/CrearAdmin (solo Admin)
        [HttpGet]
        [AuthFilter]
        public ActionResult CrearAdmin()
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index", "Eventos");

            return View();
        }

        // POST: Usuarios/CrearAdmin (solo Admin)
        [HttpPost]
        [AuthFilter]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAdmin(string Nombre, string Correo, string Contrasena, string ConfirmarContrasena)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index", "Eventos");

            if (string.IsNullOrWhiteSpace(Nombre))
                ViewBag.ErrorNombre = "El nombre es obligatorio.";
            else if (Nombre.Trim().Length < 3)
                ViewBag.ErrorNombre = "El nombre debe tener al menos 3 caracteres.";

            if (string.IsNullOrWhiteSpace(Correo))
                ViewBag.ErrorCorreo = "El correo es obligatorio.";
            else if (!Correo.Contains("@") || !Correo.Contains("."))
                ViewBag.ErrorCorreo = "El correo no tiene un formato válido.";

            if (string.IsNullOrWhiteSpace(Contrasena))
                ViewBag.ErrorContrasena = "La contraseña es obligatoria.";
            else if (Contrasena.Length < 6)
                ViewBag.ErrorContrasena = "La contraseña debe tener al menos 6 caracteres.";
            else if (Contrasena != ConfirmarContrasena)
                ViewBag.ErrorContrasena = "Las contraseñas no coinciden.";

            if (ViewBag.ErrorNombre != null || ViewBag.ErrorCorreo != null || ViewBag.ErrorContrasena != null)
                return View();

            Usuario nuevoAdmin = new Usuario();
            nuevoAdmin.Nombre = Nombre.Trim();
            nuevoAdmin.Correo = Correo.Trim();

            int creadoPorId = Convert.ToInt32(Session["UsuarioId"]);
            bool creado = usuarioService.RegistrarAdministrador(nuevoAdmin, Contrasena, creadoPorId);

            if (!creado)
            {
                ViewBag.ErrorCorreo = "El correo ya está registrado.";
                return View();
            }

            TempData["Mensaje"] = "Administrador creado exitosamente.";
            return RedirectToAction("Index");
        }

        // POST: Usuarios/CambiarEstado (solo Admin)
        [HttpPost]
        [AuthFilter]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, bool estado)
        {
            if (Session["UsuarioRol"] == null || Session["UsuarioRol"].ToString() != "Admin")
                return RedirectToAction("Index", "Eventos");

            usuarioService.CambiarEstado(id, estado);
            TempData["Mensaje"] = "Estado del usuario actualizado.";
            return RedirectToAction("Index");
        }
    }
}