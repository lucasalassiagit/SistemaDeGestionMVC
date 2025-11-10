using CrudMVCApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.ViewModels
{
    //ViewModel: es una clase personalizada que se crea para representar los datos que necesita una vista
    public class PedidoViewModel //Se utiliza en la creacion de un pedido
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int PersonaId { get; set; }

        public List<DetallePedidoViewModel> Detalles { get; set; } = new();

        public int UsuarioId{ get; set; }

        [Display(Name = "Usuario")]
        public string UsuarioNombre { get; set; }

        public int TotalProductos => Detalles?.Count ?? 0;

        public double Total => Detalles != null ? Detalles.Sum(d => d.Subtotal) : 0;
    }

    public class DetallePedidoViewModel //En la creacion del detalle
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Subtotal => Cantidad * PrecioUnitario;
    }

    public class MostrarDetalleViewModel //Se utiliza para mostrar el detalle de un pedido ya creado
    {
        public Pedido Pedido { get; set; }

        public List<DetallePedido> Detalles { get; set; }
        public List<DetallePedidoViewModel> DetalleTotal { get; set; }
        public double Total => DetalleTotal?.Sum(d => d.Subtotal) ?? 0;
    }
}

