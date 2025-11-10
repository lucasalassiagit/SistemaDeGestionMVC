using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; } // Clave foránea para el usuario que realiza el pedido
        public Usuario Usuario { get; set; } // Propiedad de navegación para el usuario

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int PersonaId { get; set; } 
        public Persona Persona { get; set; }

        [Display(Name = "Fecha del Pedido")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe indicar el nombre del usuario que realiza el pedido")]
        [Display(Name = "Usuario que realiza el pedido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no debe superar los 50 caracteres")]
        public ICollection<DetallePedido> Detalles { get; set; }
    }
}
