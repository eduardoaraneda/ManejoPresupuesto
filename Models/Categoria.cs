using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo es Requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        [Display(Name ="Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; }
    }
}
