
namespace ManejoPresupuesto.Servicios
{
    public interface IServicioEmail
    {
        Task EnviarEmail(string receptor, string enlace);
    }
}