using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class OlvideMiPassword
    {
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Email { get; set; }
    }
}
