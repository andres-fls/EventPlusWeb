using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlusWeb1.Models.Entities
{
    public class Evento
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEvento { get; set; }
        public string Ubicacion { get; set; }
        public int CategoriaId { get; set; }
        public int UsuarioCreadorId { get; set; }

        // Propiedades de navegación (para mostrar nombres en vez de IDs)
        public string CategoriaNombre { get; set; }
        public string CreadorNombre { get; set; }
    }
}