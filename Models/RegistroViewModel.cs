using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Password { get; set; }
    }
}
