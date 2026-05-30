using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlusWeb1.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
    }
}