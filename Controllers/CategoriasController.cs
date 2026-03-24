using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios repositoriousuarios;
        public CategoriasController(IRepositorioCategorias repositorioCategorias, IServicioUsuarios repositoriousuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.repositoriousuarios = repositoriousuarios;
        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(ManejoPresupuesto.Models.Categoria categoria)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            await repositorioCategorias.Crear(categoria);

            // 👇 Guardamos el mensaje temporal
            TempData["MensajeExito"] = "La categoria fue creada exitosamente.";

            return RedirectToAction("Index", "Categorias");
        }
        public async Task<IActionResult> Index(PaginacionViewModel paginacionViewModel)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();

            var categorias = await repositorioCategorias.ObtenerCategorias(usuarioId, paginacionViewModel);
            var totalCategorias = await repositorioCategorias.Contar(usuarioId);

            var respuestaVM = new PaginacionRespuesta<Categoria>
            {
                Elementos = categorias,
                Pagina = paginacionViewModel.Pagina,
                RecordsPorPagina = paginacionViewModel.RecordsPorPagina,
                TotalRecords = totalCategorias,
                UrlBase = Url.Action("Index", "Categorias")
            };

            return View(respuestaVM);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(ManejoPresupuesto.Models.Categoria categoria)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();
            var categoriaExiste = await repositorioCategorias.ObtenerPorId(categoria.Id, usuarioId);
            if (categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Editar(categoria);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }
        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = repositoriousuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCategorias.Eliminar(id);
            return RedirectToAction("Index");
        }

    }
}