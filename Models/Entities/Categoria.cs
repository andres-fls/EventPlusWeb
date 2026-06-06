using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Categoria
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(45, ErrorMessage = "Máximo 45 caracteres")]
        [Display(Name = "Nombre de Categoría")]
        public string NombreCategoria { get; set; }
    }
}
