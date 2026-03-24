using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReportes servicioReportes;

        public TransaccionesController(IServicioUsuarios servicioUsuarios, IRepositorioCuenta repositorioCuenta, IRepositorioCategorias repositorioCategorias,
            IRepositorioTransacciones repositorioTransacciones, IMapper mapper, IServicioReportes servicioReportes)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;
        }
        [Authorize]
        public async Task<ActionResult> Index(int mes, int año)
        {
            ViewBag.Mes = mes;
            ViewBag.Ano = año;
            var usuarioid = servicioUsuarios.ObtenerUsuarioId();
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes == 0) mes = DateTime.Today.Month;
            if (año == 0) año = DateTime.Today.Year;

            if (mes <= 0 || mes > 12 || año < 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);

            }
            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioid,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);


            var modelo = new ReporteTransaccionesDetallada();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetallada.transaccionesPorFecha
                {
                    FechaTransaccion = grupo.Key,
                    transaccions = grupo.AsEnumerable()
                });

            modelo.transaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.anoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesSiguiente = fechaInicio.AddMonths(1).Month;
            ViewBag.anoSiguiente = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            return View(modelo);
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new Models.TransaccionCreacionViewModel();
            modelo.Cuentas = await obtenerCuentas(usuarioId);
            modelo.Categorias = await obtenerCategorias(usuarioId, modelo.tipoOperacionId);
            return View(modelo);
        }

        private async Task<IEnumerable<SelectListItem>> obtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuenta.Obtener(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
        [HttpPost]
        public async Task<IActionResult> ObtenerCategoria([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await obtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);

        }
        private async Task<IEnumerable<SelectListItem>> obtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.ObtenerCategorias2(usuarioId, tipoOperacion);
            var resultado = categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
            var opcionPorDefecto = new SelectListItem("Seleccione una categoria", "0", true);
            resultado.Insert(0, opcionPorDefecto);
            return resultado;
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Models.TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await obtenerCuentas(usuarioId);
                modelo.Categorias = await obtenerCategorias(usuarioId, modelo.tipoOperacionId);
                return View(modelo);
            }
            var cuenta = await repositorioCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            modelo.UsuarioId = usuarioId;
            if (modelo.tipoOperacionId == TipoOperacion.gasto)
            {
                modelo.Monto = modelo.Monto * -1;
            }
            await repositorioTransacciones.Crear(modelo);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<Models.TransaccionActualizacionViewModel>(transaccion);
            if (modelo.tipoOperacionId == TipoOperacion.gasto)
            {
                modelo.Monto = modelo.Monto * -1;
            }
            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await obtenerCategorias(usuarioId, modelo.tipoOperacionId);
            modelo.Cuentas = await obtenerCuentas(usuarioId);
            modelo.urlRetorno = urlRetorno;

            return View(modelo);


        }
        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel model)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                model.Cuentas = await obtenerCuentas(usuarioId);
                model.Categorias = await obtenerCategorias(usuarioId, model.tipoOperacionId);
                return View(model);
            }
            var transaccion = mapper.Map<Transaccion>(model);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var cuenta = await repositorioCuenta.ObtenerPorId(model.CuentaId, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var categoria = await repositorioCategorias.ObtenerPorId(model.CategoriaId, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            model.MontoAnterior = model.Monto;

            if (model.tipoOperacionId == TipoOperacion.gasto)
            {
                model.Monto = model.Monto * -1;
            }


            await repositorioTransacciones.Actualizar(model, model.MontoAnterior, model.CuentaAnteriorId);

            TempData["Mensaje"] = "La transaccion se actualizo correctamente";

            if (string.IsNullOrEmpty(model.urlRetorno))
            {
                return RedirectToAction("Index");
            } else
            {
                return LocalRedirect(model.urlRetorno);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTransacciones.Borrar(id);
            TempData["Mensaje"] = "La transaccion se elimino correctamente";

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }

        public async Task<IActionResult> Detalle(int mes, int ano)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetallada(usuarioId, mes, ano, ViewBag);

            return View("_ReporteTransaccionesDetallada", modelo);


        }
        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            // Normalizar mes/año si vienen en cero
            if (mes == 0) mes = DateTime.Today.Month;
            if (año == 0) año = DateTime.Today.Year;

            // 🔥 MUY IMPORTANTE: agregar esto para que el Submenu NO se borre
            ViewBag.Mes = mes;
            ViewBag.Ano = año;

            var transaccionesPorSemana = await servicioReportes.ObtenerPorSemana(usuarioId, mes, año, ViewBag);

            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemanaEnumerable =
                transaccionesPorSemana as IEnumerable<ResultadoObtenerPorSemana>;

            if (transaccionesPorSemanaEnumerable == null)
            {
                throw new InvalidCastException("transaccionesPorSemana no se puede convertir a IEnumerable<ResultadoObtenerPorSemana>.");
            }

            var agrupado = transaccionesPorSemanaEnumerable.GroupBy(x => x.Semana)
                .Select(x => new ResultadoObtenerPorSemana()
                {
                    Semana = x.Key,
                    ingresos = x.Where(y => y.tipoOperacionId == TipoOperacion.ingreso).Select(y => y.Monto).FirstOrDefault(),
                    gastos = x.Where(y => y.tipoOperacionId == TipoOperacion.gasto).Select(y => y.Monto).FirstOrDefault()
                }).ToList();

            // Construir fechas por semana
            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for (int i = 0; i < diasSegmentados.Count; i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());

                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if (grupoSemana == null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        fechaInicio = fechaInicio,
                        fechaFin = fechaFin
                    });
                }
                else
                {
                    grupoSemana.fechaInicio = fechaInicio;
                    grupoSemana.fechaFin = fechaFin;
                }
            }

            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanalViewModel()
            {
                ResultadoObtenerPorSemana = agrupado,
                FechaReferencia = fechaReferencia
            };

            return View(modelo);
        }


        public async Task<IActionResult> Mensual(int ano)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (ano == 0) ano = DateTime.Today.Year;
            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioId, ano);

            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtenerPorMes()
                {
                    Mes = x.Key,
                    Ingresos = x.Where(y => y.TipoOperacionId == TipoOperacion.ingreso).Select(y => y.Monto).FirstOrDefault(),
                    Egresos = x.Where(y => y.TipoOperacionId == TipoOperacion.gasto).Select(y => y.Monto).FirstOrDefault()
                }).ToList();

            for (int mes = 1; mes <= 12; mes++)
            {
                var grupoMes = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(ano, mes, 1);
                if (grupoMes == null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = new DateTime(ano, mes, 1)
                    });
                }
                else
                {
                    grupoMes.FechaReferencia = new DateTime(ano, mes, 1);
                }
            }
            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();
            var modelo = new ReporteMensualViewModel()
            {
                TransaccionePorMes = transaccionesAgrupadas,
                ano = ano
            };

            return View(modelo);
        }
        public IActionResult ExcelReporte(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcel(int mes, int año)
        {
            var fechaInicio = new DateTime(año, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin
            });

            var archivoExcel = $"Manejo Presupuesto - {fechaInicio.ToString("MMM yyyy")}.xlsx";

            return GenerarExcelDesdeTransacciones(transacciones, archivoExcel);
        }

        private FileResult GenerarExcelDesdeTransacciones(IEnumerable<Transaccion> transacciones, string nombre)
        {
            DataTable tabla = new DataTable("Transacciones");
            tabla.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("FechaTransaccion", typeof(DateTime)),
                new DataColumn("Cuenta", typeof(string)),
                new DataColumn("Categoria", typeof(string)),
                new DataColumn("Nota", typeof(string)),
                new DataColumn("Monto", typeof(string)),
                new DataColumn("Ingreso/Gasto", typeof(string))
            });

            foreach (var transaccion in transacciones)
            {
                tabla.Rows.Add(transaccion.FechaTransaccion, transaccion.Cuenta, transaccion.Categoria, transaccion.Nota, transaccion.Monto,
                    transaccion.tipoOperacionId);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(tabla);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombre);
                }
            }
        }
        [HttpGet]
        public async Task<FileResult> ExportarExcelPorAño(int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var fechaInicio = new DateTime(año, 1, 1);
            var fechaFin = new DateTime(año, 12, 31);
            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin
            });

            var nombreArchivo = $"Manejo Presupuesto - {año}.xlsx";
            return GenerarExcelDesdeTransacciones(transacciones, nombreArchivo);
        }
        [HttpGet]
        public async Task<FileResult> ExportarExcelTodo()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(1);

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin
            });

            var nombreArchivo = $"Manejo Presupuesto - Todo.xlsx";
            return GenerarExcelDesdeTransacciones(transacciones, nombreArchivo);
        }
        public IActionResult Calendario(int id)
        {
            return View();
        }

        public async Task<JsonResult> ObtenerTransaccionesCalendario(DateTime start, DateTime end)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                fechaInicio = start,
                fechaFin = end
            });

            var eventos = transacciones.Select(transaccion => new EventoCalendario()
            {
                Title = transaccion.Monto.ToString("N"),
                Start = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                End = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                Color = (transaccion.tipoOperacionId == TipoOperacion.gasto) ? "Red" : "Green"
            });

            return Json(eventos);
        }
        public async Task<JsonResult> ObtenerTransaccionesPorFecha(DateTime fecha)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                fechaInicio = fecha,
                fechaFin = fecha
            });
            return Json(transacciones);

        }

    }
}