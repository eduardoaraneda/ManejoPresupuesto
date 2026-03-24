namespace ManejoPresupuesto.Models
{
    public class ReporteSemanalViewModel 
    {
        public decimal Ingresos => ResultadoObtenerPorSemana.Sum(x => x.ingresos);
        public decimal Gastos => ResultadoObtenerPorSemana.Sum(x => x.gastos);
        public decimal Total { get; set; }
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana> ResultadoObtenerPorSemana { get; set; }

    }
}
