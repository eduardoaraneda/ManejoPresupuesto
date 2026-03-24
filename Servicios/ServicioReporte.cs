using ManejoPresupuesto.Models;
using System.Runtime.CompilerServices;

namespace ManejoPresupuesto.Servicios
{
    
    public interface IServicioReportes
    {
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(int usuarioid, int mes, int ano, dynamic ViewBag);
        Task<ReporteTransaccionesDetallada> ObtenerReporteTransaccionesDetallada(int usuarioId, int mes, int ano, dynamic ViewBag);
        Task<ReporteTransaccionesDetallada> ObtenerReporteTransaccionesDetalladaPorCuenta(
           int usuarioId, int cuentaId, int mes, int ano, dynamic viewBag);
    }
    public class ServicioReporte : IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;

        public ServicioReporte(IRepositorioTransacciones repositorioTransacciones, IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<ReporteTransaccionesDetallada> ObtenerReporteTransaccionesDetalladaPorCuenta(int usuarioId, int cuentaId, int mes, int ano, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFecha(mes, ano);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporteTransaccionesDetallada(FechaInicio, FechaFin, transacciones);
            ViewBag.IdCuenta = cuentaId;
            AsignarValoresALViewbag( ViewBag, FechaInicio);

            return modelo;

        }

        private void AsignarValoresALViewbag(dynamic ViewBag, DateTime FechaInicio)
        {
            
            ViewBag.mesAnterior = FechaInicio.AddMonths(-1).Month;
            ViewBag.anoAnterior = FechaInicio.AddMonths(-1).Year;
            ViewBag.mesSiguiente = FechaInicio.AddMonths(1).Month;
            ViewBag.anoSiguiente = FechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetallada GenerarReporteTransaccionesDetallada(DateTime FechaInicio, DateTime FechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetallada();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetallada.transaccionesPorFecha
                {
                    FechaTransaccion = grupo.Key,
                    transaccions = grupo.AsEnumerable()
                });

            modelo.transaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = FechaInicio;
            modelo.FechaFin = FechaFin;


            return modelo;
        }

        public async Task<ReporteTransaccionesDetallada> ObtenerReporteTransaccionesDetallada(int usuarioId, int mes, int ano, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFecha(mes, ano);

            var obtenerTransaccionesPorCuenta = new ParametroObtenerTransaccionesPorUsuario()
            {

                UsuarioId = usuarioId,
                fechaInicio = FechaInicio,
                fechaFin = FechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporteTransaccionesDetallada(FechaInicio, FechaFin, transacciones);

            AsignarValoresALViewbag(ViewBag, FechaInicio);
            return modelo;

        }

        private (DateTime FechaInicio, DateTime FechaFin) GenerarFecha(int mes, int ano)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || ano < 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(ano, mes, 1);

            }
            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(int usuarioid, int mes, int ano, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFecha(mes, ano);

            var obtenerTransaccionesPorCuenta = new ParametroObtenerTransaccionesPorUsuario()
            {

                UsuarioId = usuarioid,
                fechaInicio = FechaInicio,
                fechaFin = FechaFin
            };

            AsignarValoresALViewbag(ViewBag, FechaInicio);

            var modelo = await repositorioTransacciones.ObtenerPorSemana(obtenerTransaccionesPorCuenta);
            return modelo;
        }
    }
}
