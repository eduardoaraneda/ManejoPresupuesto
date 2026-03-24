using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuenta
    {
        Task Crear(Cuenta Cuenta);
        Task<IEnumerable<Cuenta>> Obtener(int usuarioId);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
        Task<Cuenta> Editar(Cuenta cuenta);
        Task Eliminar(int id);
    }
    public class ReporsitorioCuenta : IRepositorioCuenta
    {
        private readonly string connectionString;
        public ReporsitorioCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta Cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Balance, descripcion)
                                                        VALUES (@Nombre, @TipoCuentaId, @Balance, @descripcion);
                                                        SELECT SCOPE_IDENTITY();", Cuenta);
            Cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT c.Id, c.Nombre, c.TipoCuentaId, c.Balance, tc.Nombre AS TipoCuenta
                                                         FROM Cuentas c
                                                         INNER JOIN TiposCuentas tc ON c.TipoCuentaId = tc.id
                                                         WHERE tc.UsuarioId = @UsuarioId Order By tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT c.Id, c.Nombre, c.TipoCuentaId, c.Balance, TipoCuentaId
                                                         FROM Cuentas c
                                                         INNER JOIN TiposCuentas tc ON c.TipoCuentaId = tc.id
                                                         WHERE c.Id = @Id AND tc.UsuarioId = @UsuarioId", new { id, usuarioId });
        }
        public async Task<Cuenta> Editar(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                             SET Nombre = @Nombre,
                                                 Balance = @Balance,
                                                 TipoCuentaId = @TipoCuentaId,
                                                 descripcion = @descripcion
                                             WHERE Id = @Id", cuenta);
            return cuenta;
        }
        public async Task Eliminar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM Cuentas WHERE Id = @Id", new { id });
           
        }
    }
}