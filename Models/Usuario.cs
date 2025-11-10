using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Debe ingresar usuario")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string user { get; set; }

        [Display(Name = "Clave")]
        [Required(ErrorMessage = "Debe ingresar la clave")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$",
        ErrorMessage = "La clave debe tener al menos 6 caracteres, una letra y un número")]
        public string Clave { get; set; } // En sistemas reales, esto debería ser un hash

        [Display(Name = "Tipo de Usuario")]
        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        public string Tipo { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public Usuario() { }
    
    }

}
