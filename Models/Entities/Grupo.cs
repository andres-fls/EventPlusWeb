using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Grupo
    {
        public int IdGrupo { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        [Display(Name = "Evento")]
        public int Evento_idEvento { get; set; }

        [Required(ErrorMessage = "El nombre del grupo es obligatorio")]
        [StringLength(45, ErrorMessage = "Máximo 45 caracteres")]
        [Display(Name = "Nombre del Grupo")]
        public string NombreGrupo { get; set; }

        // Propiedad extra para mostrar en vistas (JOIN con Evento)
        [Display(Name = "Evento")]
        public string NombreEvento { get; set; }
    }
}
