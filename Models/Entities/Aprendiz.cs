using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Aprendiz
    {
        public int IdAprendiz { get; set; }

        [Required]
        public int Usuario_idUsuario { get; set; }

        [Required(ErrorMessage = "La ficha es obligatoria")]
        [Display(Name = "Ficha")]
        public int Ficha_idFicha { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Edad")]
        [Range(14, 99, ErrorMessage = "La edad debe estar entre 14 y 99")]
        public int? Edad { get; set; }

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        // Propiedades extra para mostrar en vistas (JOINs)
        [Display(Name = "Nombre")]
        public string NombreUsuario { get; set; }

        [Display(Name = "Ficha")]
        public string CodigoFicha { get; set; }
    }
}