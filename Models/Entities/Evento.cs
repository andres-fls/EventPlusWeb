using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Evento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha del evento es obligatoria")]
        public DateTime FechaEvento { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        public int UsuarioCreadorId { get; set; }

        // Propiedades de navegación (para mostrar nombres en vez de IDs)
        public string CategoriaNombre { get; set; }
        public string CreadorNombre { get; set; }
    }
}