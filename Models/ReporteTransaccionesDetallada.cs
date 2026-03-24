namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetallada
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<transaccionesPorFecha> transaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => transaccionesAgrupadas.Sum(x => x.BalanceDepoditos);
        public decimal BalanceRetiros => transaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal BalanceTotal => BalanceDepositos - BalanceRetiros;
        public class transaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> transaccions { get; set; }
            public decimal BalanceDepoditos => transaccions.Where(x => x.tipoOperacionId == TipoOperacion.ingreso).Sum(x => x.Monto);
            public decimal BalanceRetiros => transaccions.Where(x => x.tipoOperacionId == TipoOperacion.gasto).Sum(x => x.Monto);
        }
    }
}
