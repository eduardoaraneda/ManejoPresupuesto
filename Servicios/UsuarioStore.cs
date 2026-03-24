using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Servicios
{
    public class UsuarioStore :
        IUserStore<Usuario>,
        IUserEmailStore<Usuario>,
        IUserPasswordStore<Usuario>
    {
        private readonly IRepositorioUsuarios repositorioUsuarios;

        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios)
        {
            this.repositorioUsuarios = repositorioUsuarios;
        }

        // -----------------------------------------------------
        // CREATE USER
        // -----------------------------------------------------
        public async Task<IdentityResult> CreateAsync(Usuario user, CancellationToken cancellationToken)
        {
            user.Id = await repositorioUsuarios.CrearUsuarioAsync(user);
            return IdentityResult.Success;
        }

        // -----------------------------------------------------
        // DELETE USER
        // -----------------------------------------------------
        public async Task<IdentityResult> DeleteAsync(Usuario user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------------
        // DISPOSE (NO NECESITA IMPLEMENTACIÓN)
        // -----------------------------------------------------
        public void Dispose()
        {
            // No se requieren recursos a liberar
        }

        // -----------------------------------------------------
        // FIND BY ID
        // -----------------------------------------------------
        public async Task<Usuario> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // -----------------------------------------------------
        // FIND BY EMAIL
        // -----------------------------------------------------
        public async Task<Usuario> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        // -----------------------------------------------------
        // FIND BY USERNAME (usa email como username)
        // -----------------------------------------------------
        public async Task<Usuario> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedUserName);
        }

        // -----------------------------------------------------
        // GETTERS
        // -----------------------------------------------------
        public Task<string> GetEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true); // Asume siempre confirmado
        }

        public Task<string> GetNormalizedEmailAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalizado);
        }

        public Task<string> GetNormalizedUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalizado);
        }

        public Task<string> GetPasswordHashAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(Usuario user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        // -----------------------------------------------------
        // SETTERS
        // -----------------------------------------------------
        public Task SetEmailAsync(Usuario user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(Usuario user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(Usuario user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(Usuario user, string normalizedName, CancellationToken cancellationToken)
        {
            user.EmailNormalizado = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuario user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuario user, string userName, CancellationToken cancellationToken)
        {
            user.Email = userName;
            return Task.CompletedTask;
        }

        // -----------------------------------------------------
        // UPDATE USER
        // -----------------------------------------------------
        public async Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken)
        {
            await repositorioUsuarios.Actualizar(user);
            return IdentityResult.Success;
        }
    }
}
