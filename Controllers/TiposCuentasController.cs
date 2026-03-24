using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers

{
    public class TiposCuentasController : Controller
    {
        private readonly string connectionString;
        private readonly IServicioUsuarios servicioUsuarios;


        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            RepositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public IRepositorioTiposCuentas RepositorioTiposCuentas { get; }

        public IActionResult Crear()
        {

            return View();
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await RepositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existeTipoCuenta = await RepositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if (existeTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El tipo de cuenta {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await RepositorioTiposCuentas.Crear(tipoCuenta);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await RepositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("No Encontrado", "Home");
            }
            return View(tipoCuenta);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await RepositorioTiposCuentas.ObtenerPorId(tipoCuenta.id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("No encontrado", "Home");
            }
            await RepositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre, int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var existeTipoCuenta = await RepositorioTiposCuentas.Existe(nombre, usuarioId, id);
            if (existeTipoCuenta)
            {
                return Json($"El tipo de cuenta {nombre} ya existe.");
            }
            return Json(true);
        }
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await RepositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("No encontrado", "Home");
            }
            return View(tipoCuenta);
        }
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await RepositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("No encontrado", "Home");
            }
            await RepositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuario = servicioUsuarios.ObtenerUsuarioId();
            var tiposcuentas = await RepositorioTiposCuentas.Obtener(usuario);
            var idstipos = tiposcuentas.Select(x => x.id);

            var idsTiposCuentasNoPertenece = ids.Except(idstipos).ToList();

            if (idsTiposCuentasNoPertenece.Count > 0)
            {
                return Forbid();
            }            

            var tiposCuentasOrdenados = ids.Select((valor,indice) => new TipoCuenta() { id = valor, Orden = indice + 1 } ).AsEnumerable();

            await RepositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }

    }
}
