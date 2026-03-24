namespace ManejoPresupuesto.Models
{
    public class ObtenerTransaccionesPorCuenta
    {
        public int CuentaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

    }
}
