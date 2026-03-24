using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        [Display(Name = "Fecha de Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today; /*DateTime.Parse(DateTime.Now.ToString("g"));*/
        [Range(0, int.MaxValue, ErrorMessage = "Seleccione una categoria")]
        public int CategoriaId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int CuentaId { get; set; }
        public int UsuarioId { get; set; }
        [MaxLength(200, ErrorMessage = "La nota no puede tener más de 200 caracteres")]
        public string Nota { get; set; }
        public TipoOperacion tipoOperacionId { get; set; } = TipoOperacion.ingreso;
        public string Categoria { get; set; }
        public string Cuenta { get; set; }
    }
}

