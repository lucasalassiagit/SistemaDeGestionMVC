using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Direccion
    {
        public int Id { get; set; } // Clave primaria necesaria para EF

        [Required(ErrorMessage = "El campo Calle es obligatorio")]
        [StringLength(150, ErrorMessage = "La Calle no puede tener más de 150 caracteres")] 
        public string Calle { get; set; }
        
        [Required(ErrorMessage = "El campo Ciudad es obligatorio")]
        [MinLength(3, ErrorMessage = "La Ciudad no puede tener menos 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "La Ciudad no puede tener más de 20 caracteres.")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El Codigo Postal es obligatorio")]
        [Display(Name = "Código Postal")]
        [RegularExpression(@"^\d{4,8}$", ErrorMessage = "El Código Postal debe contener solo números (entre 4 y 8 dígitos)")]
        public string CodigoPostal { get; set; }

        // Clave Foranea
        [Required]
        public int PersonaId { get; set; }

        // propiedad de navegación
        [ValidateNever]
        public Persona? Persona { get; set; }

        public Direccion() { }
    }
}
