using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Ficha
    {
        public int IdFicha { get; set; }

        [Required(ErrorMessage = "El programa es obligatorio")]
        [Display(Name = "Programa")]
        public int Programa_idPrograma { get; set; }

        [Required(ErrorMessage = "El código de ficha es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Código de Ficha")]
        public string CodigoFicha { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        // Propiedad extra para mostrar en vistas (JOIN con Programa)
        [Display(Name = "Programa")]
        public string NombrePrograma { get; set; }
    }
}