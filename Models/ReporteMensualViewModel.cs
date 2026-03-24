namespace ManejoPresupuesto.Models
{
    public class ReporteMensualViewModel
    {
        public IEnumerable<ResultadoObtenerPorMes> TransaccionePorMes { get; set; }
        public decimal Ingresos => TransaccionePorMes.Sum(x => x.Ingresos);
        public decimal Egresos => TransaccionePorMes.Sum(x => x.Egresos);
        public decimal Total => Ingresos - Egresos;
        public int ano { get; set; }

        }
}
