using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Inscripcion
    {
        public int IdInscripcion { get; set; }

        [Display(Name = "Grupo")]
        public int? Grupo_idGrupo { get; set; }

        [Required(ErrorMessage = "El aprendiz es obligatorio")]
        [Display(Name = "Aprendiz")]
        public int Aprendiz_idAprendiz { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        [Display(Name = "Evento")]
        public int Evento_idEvento { get; set; }

        [Display(Name = "Fecha de Inscripción")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInscripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Estado")]
        public string EstadoInscripcion { get; set; }

        // Propiedades extra para mostrar en vistas (JOINs)
        [Display(Name = "Aprendiz")]
        public string NombreAprendiz { get; set; }

        [Display(Name = "Evento")]
        public string NombreEvento { get; set; }

        [Display(Name = "Grupo")]
        public string NombreGrupo { get; set; }
    }
}