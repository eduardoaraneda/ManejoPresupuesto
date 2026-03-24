using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, PaginacionViewModel paginacionViewModel);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
        Task<Categoria> Editar(Categoria categoria);
        Task Eliminar(int id);
        Task<IEnumerable<Categoria>> ObtenerCategorias2(int usuarioId, TipoOperacion tipoOperacion);
        Task<int> Contar(int usuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                        VALUES (@Nombre, @TipoOperacionId, @UsuarioId);
                                                        SELECT SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }
        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId, PaginacionViewModel paginacionViewModel)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@$"SELECT * FROM Categorias
                                                            WHERE UsuarioId = @UsuarioId
                                                            ORDER BY Nombre
                                                            OFFSET {paginacionViewModel.Skip} ROWS FETCH NEXT {paginacionViewModel.RecordsPorPagina} ROWS ONLY                                        
                                                            ", new {  usuarioId});
        }
        public async Task<int> Contar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleAsync<int>(@"SELECT COUNT(*) FROM Categorias
                                                            WHERE UsuarioId = @UsuarioId", new { usuarioId });

        }

        public async Task<IEnumerable<Categoria>> ObtenerCategorias2(int usuarioId, TipoOperacion tipoOperacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@"SELECT * FROM Categorias
                                                            WHERE UsuarioId = @UsuarioId And TipoOperacionId = @TipoOperacion
                                                            ORDER BY Nombre", new { usuarioId, tipoOperacion });
        }
        public async Task Eliminar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM Categorias
                                             WHERE Id = @Id", new { id });
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"SELECT * FROM Categorias
                                                                          WHERE Id = @Id AND UsuarioId = @UsuarioId", new { id, usuarioId });
        }
        public async Task<Categoria> Editar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias
                                             SET Nombre = @Nombre,
                                                 TipoOperacionId = @TipoOperacionId
                                             WHERE Id = @Id", categoria);
            return categoria;
        }
    }
}
