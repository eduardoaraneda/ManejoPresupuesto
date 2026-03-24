namespace ManejoPresupuesto.Models
{
    public class ResultadoObtenerPorSemana
    {
        public int Semana { get; set; }
        public decimal Monto { get; set; }
        public TipoOperacion tipoOperacionId { get; set; }
        public decimal ingresos { get; set; }
        public decimal gastos { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }
}
