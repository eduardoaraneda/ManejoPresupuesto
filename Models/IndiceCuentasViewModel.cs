namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string tipoCuenta { get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set; }
        public decimal BalanceTotal => Cuentas.Sum(c => c.Balance);
    }
}
