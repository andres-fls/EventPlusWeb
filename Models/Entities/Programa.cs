using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Programa
    {
        public int IdPrograma { get; set; }

        [Required(ErrorMessage = "El código del programa es obligatorio")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        [Display(Name = "Código del Programa")]
        public string CodigoPrograma { get; set; }

        [Required(ErrorMessage = "El nombre del programa es obligatorio")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        [Display(Name = "Nombre del Programa")]
        public string NombrePrograma { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Display(Name = "Duración (Meses)")]
        [Range(1, 60, ErrorMessage = "La duración debe estar entre 1 y 60 meses")]
        public int DuracionMeses { get; set; }

        [Required(ErrorMessage = "El nivel del programa es obligatorio")]
        [StringLength(45, ErrorMessage = "Máximo 45 caracteres")]
        [Display(Name = "Nivel del Programa")]
        public string NivelPrograma { get; set; }
    }
}
