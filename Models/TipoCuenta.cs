using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta //: IValidatableObject
    {
        public int id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido")]
        [PrimeraLetraMayus]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public int UsuarioId { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayúscula", new[] { nameof(Nombre) });
        //        }
        //    }
        //}
    }
}
