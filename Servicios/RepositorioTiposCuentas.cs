using Microsoft.Data.SqlClient;
using Dapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentasInsertar", new {usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre},
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                           FROM TiposCuentas
                                                                           WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId AND id <> @id;",
                                                                           new { nombre, usuarioId, id });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT id, Nombre, Orden
                                                             FROM TiposCuentas
                                                             WHERE UsuarioId = @UsuarioId
                                                             ORDER BY Orden", new { usuarioId });
        }

        async Task IRepositorioTiposCuentas.Actualizar (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE id = @id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT id, Nombre, Orden, UsuarioId
                                                                          FROM TiposCuentas
                                                                          WHERE id = @id AND UsuarioId = @UsuarioId;",
                                                                          new { id, usuarioId });
        }
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas
                                            WHERE id = @id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden where Id = @Id";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync (query, tipoCuentas);
        }

       
    }
}