using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlusWeb1.Models.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        [Display(Name = "Contraseña")]
        public string ContrasenaHash { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Rol")]
        public string Rol { get; set; }

        public bool Estado { get; set; }
    }
}
