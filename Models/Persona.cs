using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CrudMVCApp.Models
{
    public class Persona //Cliente
    {
        public int Id { get; set; } // Clave primaria necesaria para EF

        [Display(Name = "Nombre del Cliente")]
        [Required(ErrorMessage = "Debe ingresar el nombre")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombre { get; set; }
       
        [Display(Name = "Apellido del Cliente")]
        [Required(ErrorMessage = "Debe ingresar el apellido")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Apellido { get; set; }

        [Display(Name = "DNI")]
        [Required(ErrorMessage = "Debe ingresar el DNI")]
        [Range(1000000, 99999999, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos")]
        public int Dni { get; set; }

        [Display(Name = "CUIT (Sin guiones ni puntos)")]
        [Required(ErrorMessage = "Debe ingresar el CUIT")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener exactamente 11 dígitos numéricos")]
        public string Cuit { get; set; }
        public bool Efectivo { get; set; }
        public bool Tarjeta { get; set; }
        public bool Transferencia { get; set; }

        [Display(Name = "Género")]
        [Required(ErrorMessage = "Debe seleccionar un género")]
        public char Genero { get; set; }
        public bool Activo { get; set; } = true;

        //Propiedad navegacion 
        [ValidateNever]
        public ICollection<Direccion> Direcciones { get; set; }

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public Persona (){}
        
    }
}

