using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlusWeb1.Models.Entities
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EventoId { get; set; }
        public DateTime FechaInscripcion { get; set; }

        // Propiedades de navegación
        public string UsuarioNombre { get; set; }
        public string EventoTitulo { get; set; }
    }
}