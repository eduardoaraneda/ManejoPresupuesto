namespace ManejoPresupuesto.Models
{
    public class ParametroObtenerTransaccionesPorUsuario
    {
        public int UsuarioId {  get; set; }
        public DateTime fechaInicio {  get; set; }
        public DateTime fechaFin {  get; set; }
    }
}
