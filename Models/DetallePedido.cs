using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }
        [Required]
        public Pedido Pedido { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        public Producto Producto { get; set; }

        [Required(ErrorMessage = "Debe ingresar una cantidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required]
        [Display(Name = "Precio Unitario")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public double PrecioUnitario { get; set; }

        public double Subtotal => Cantidad * PrecioUnitario;

    }
}
