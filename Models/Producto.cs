using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace CrudMVCApp.Models
{
    public class Producto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria")]
        [StringLength(300, ErrorMessage = "La descripción no puede tener más de 300 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de compra es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de compra debe ser mayor a 0")]
        [Display(Name = "Precio de Compra")]
        public double PrecioCompra  { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor a 0")]
        [Display(Name = "Precio de Venta")]
        public double PrecioVta {  get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public bool Activo { get; set; } = true;
        public Producto() { }
    }
}
