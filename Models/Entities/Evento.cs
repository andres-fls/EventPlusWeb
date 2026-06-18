using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Evento
    {
        public int IdEvento { get; set; }

        [Required]
        public int Usuario_idUsuario { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        public int Categoria_idCategoria { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre del Evento")]
        public string NombreEvento { get; set; }

        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Descripción")]
        public string DescripcionEvento { get; set; }

        [Required(ErrorMessage = "El lugar es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Lugar")]
        public string LugarEvento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio del evento es obligatoria")]
        [Display(Name = "Fecha Inicio Evento")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicioEvento { get; set; }

        [Required(ErrorMessage = "La fecha de fin del evento es obligatoria")]
        [Display(Name = "Fecha Fin Evento")]
        [DataType(DataType.DateTime)]
        public DateTime FechaFinEvento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio de inscripción es obligatoria")]
        [Display(Name = "Inicio Inscripción")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicioInscripcion { get; set; }

        [Required(ErrorMessage = "La fecha de fin de inscripción es obligatoria")]
        [Display(Name = "Fin Inscripción")]
        [DataType(DataType.DateTime)]
        public DateTime FechaFinInscripcion { get; set; }

        [Required(ErrorMessage = "El cupo máximo es obligatorio")]
        [Display(Name = "Cupo Máximo")]
        [Range(1, 1000, ErrorMessage = "El cupo debe estar entre 1 y 1000")]
        public int CupoMaximo { get; set; }

        [Required(ErrorMessage = "El tipo de evento es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Tipo de Evento")]
        public string TipoEvento { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Estado")]
        public string EstadoEvento { get; set; }

        // Máximo de integrantes por grupo (solo eventos Grupales). Nullable.
        public int? MaxIntegrantesGrupo { get; set; }

        // Propiedades extra para mostrar en vistas (JOINs)
        [Display(Name = "Categoría")]
        public string NombreCategoria { get; set; }

        [Display(Name = "Creado por")]
        public string NombreCreador { get; set; }

        [Display(Name = "Inscritos")]
        public int Inscritos { get; set; }
    }
}