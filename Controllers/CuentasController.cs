using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuenta repositorioCuenta; // Agregar campo para la dependencia
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuenta repositorioCuenta,
            IMapper mapper, IRepositorioTransacciones repositorioTransacciones, IServicioReportes servicioReportes)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuenta = repositorioCuenta; // Asignar la dependencia
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            var modelo = new CuentaCreacionViewModel();

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);
            if (tiposCuentas is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await repositorioCuenta.Crear(cuenta); // Usar la instancia de la dependencia
            return RedirectToAction("Index", "Home");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.id.ToString()));
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentas = await repositorioCuenta.Obtener(usuarioId);
            var modelo = cuentas
                         .GroupBy(x => x.TipoCuenta)
                         .Select(grupo => new IndiceCuentasViewModel
                         {
                             tipoCuenta = grupo.Key,
                             Cuentas = grupo.AsEnumerable(),
                         }).ToList();
            return View(modelo);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);

        }
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if (!ModelState.IsValid)
            {
                cuentaEditar.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuentaEditar);
            }
            cuenta.Nombre = cuentaEditar.Nombre;
            cuenta.Balance = cuentaEditar.Balance;
            cuenta.TipoCuentaId = cuentaEditar.TipoCuentaId;
            cuenta.descripcion = cuentaEditar.descripcion;
            // Aquí deberías llamar a un método para actualizar la cuenta en el repositorio
            await repositorioCuenta.Editar(cuenta);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuenta.Eliminar(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("No encontrado", "Home");
            }
            return View(tipoCuenta);
        }
        public async Task<IActionResult> Detalle(int id, int mes, int ano)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
           
            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladaPorCuenta(usuarioId, id, mes, ano, ViewBag);

            return View(cuenta);

        }

    }
}
