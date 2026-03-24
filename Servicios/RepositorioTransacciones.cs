using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task Borrar(int id);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta filtro);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario filtro);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioid, int ano);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transaccion_Insertar", new
            {
                transaccion.Monto,
                transaccion.FechaTransaccion,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.UsuarioId,
                transaccion.Nota
            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transaccion.Id,
                transaccion.Monto,
                transaccion.FechaTransaccion,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT * FROM Transacciones WHERE Id = @Id AND UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new { id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta filtro)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId
                                                        FROM Transacciones t
                                                        inner join Categorias c on c.Id = t.CategoriaId
                                                        inner join Cuentas cu on cu.Id = t.CuentaId
                                                        where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                                                        and FechaTransaccion between @FechaInicio and @FechaFin",
                                                        filtro);
        }
        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario filtro)
        {
            using var connection = new SqlConnection( connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId, Nota
                                                        FROM Transacciones t
                                                        inner join Categorias c on c.Id = t.CategoriaId
                                                        inner join Cuentas cu on cu.Id = t.CuentaId
                                                        where  t.UsuarioId = @UsuarioId
                                                        and FechaTransaccion between @FechaInicio and @FechaFin
                                                        Order by t.FechaTransaccion Desc",
                                                        filtro);

        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT DATEPART(WEEK, FechaTransaccion) AS Semana,
                                                                            SUM(Monto) AS Monto,
                                                                            c.TipoOperacionId
                                                                        FROM Transacciones t
                                                                        INNER JOIN Categorias c ON c.Id = t.CategoriaId
                                                                        WHERE t.UsuarioId = @UsuarioId
                                                                        AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                                                        GROUP BY DATEPART(WEEK, FechaTransaccion), c.TipoOperacionId
                                                                        ORDER BY Semana",
                                                                         modelo );
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = @"
        SELECT 
            MONTH(FechaTransaccion) AS Mes,
            SUM(tr.Monto) AS Monto,
            cat.TipoOperacionId AS TipoOperacionId
        FROM Transacciones tr
        INNER JOIN Categorias cat ON cat.Id = tr.CategoriaId
        WHERE tr.UsuarioId = @usuarioId 
          AND YEAR(FechaTransaccion) = @año
        GROUP BY MONTH(FechaTransaccion), cat.TipoOperacionId";

            return await connection.QueryAsync<ResultadoObtenerPorMes>(sql,
                new { usuarioId, año });
        }

    }
}
