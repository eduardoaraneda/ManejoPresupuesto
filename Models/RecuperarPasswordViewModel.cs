using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace ManejoPresupuesto.Models
{
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string codigoreseteo { get; set; }
    }
}
