using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [PrimeraLetraMayus]
        public string Nombre { get; set; }
        [Display(Name = "Tipo de cuenta")]
        public int TipoCuentaId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(500, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Display(Name = "Descripcion")]
        public string descripcion { get; set; }
        public string TipoCuenta { get; set; }
    }
}
