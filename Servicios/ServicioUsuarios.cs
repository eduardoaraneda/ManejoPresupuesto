using System.Security.Claims;

namespace ManejoPresupuesto.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly HttpContext _httpContextAccessor;
        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor.HttpContext;
        }
        public int ObtenerUsuarioId()
        {
            if (_httpContextAccessor.User.Identity.IsAuthenticated)
            {
                var idClaim = _httpContextAccessor.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }

        }
    }
}
