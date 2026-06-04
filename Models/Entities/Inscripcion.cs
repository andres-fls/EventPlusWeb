using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Inscripcion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "La fecha de inscripción es obligatoria")]
        public DateTime FechaInscripcion { get; set; }

        // Propiedades de navegación
        public string UsuarioNombre { get; set; }
        public string EventoTitulo { get; set; }
    }
}