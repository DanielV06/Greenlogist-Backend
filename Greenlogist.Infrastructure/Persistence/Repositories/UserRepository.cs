using Greenlogist.Domain.Aggregates.User;
using Greenlogist.Domain.Repositories;
using Greenlogist.Domain.ValueObjects;
using System.Collections.Concurrent; // Para simular una base de datos en memoria

namespace Greenlogist.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación de IUserRepository que simula una base de datos en memoria.
    /// En una aplicación real, esto interactuaría con EF Core, Dapper, etc.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        // Simulación de base de datos en memoria
        private static readonly ConcurrentDictionary<Guid, User> _users = new ConcurrentDictionary<Guid, User>();
        private static readonly ConcurrentDictionary<string, Guid> _emailIndex = new ConcurrentDictionary<string, Guid>();

        public Task AddAsync(User user)
        {
            if (!_users.TryAdd(user.Id, user))
            {
                throw new InvalidOperationException($"No se pudo añadir el usuario con ID {user.Id}. Ya existe.");
            }
            if (!_emailIndex.TryAdd(user.Email.Value, user.Id))
            {
                // Esto no debería pasar si ExistsByEmailAsync se usa correctamente antes de AddAsync
                _users.TryRemove(user.Id, out _); // Rollback
                throw new InvalidOperationException($"El email '{user.Email.Value}' ya está en uso.");
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user)
        {
            _users.TryRemove(user.Id, out _);
            _emailIndex.TryRemove(user.Email.Value, out _);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            return Task.FromResult(_emailIndex.ContainsKey(email.ToLowerInvariant()));
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            if (_emailIndex.TryGetValue(email.ToLowerInvariant(), out Guid userId))
            {
                _users.TryGetValue(userId, out User? user);
                return Task.FromResult(user);
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            _users.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(User user)
        {
            // En una DB real, esto sería una operación de actualización.
            // Aquí, simplemente reemplazamos la entrada.
            if (_users.ContainsKey(user.Id))
            {
                // Antes de actualizar el usuario, si el email ha cambiado, actualizamos el índice
                var oldUser = _users[user.Id];
                if (oldUser.Email.Value != user.Email.Value)
                {
                    _emailIndex.TryRemove(oldUser.Email.Value, out _);
                    if (!_emailIndex.TryAdd(user.Email.Value, user.Id))
                    {
                        // Manejar el caso de colisión de email si ocurre durante la actualización
                        throw new InvalidOperationException($"El nuevo email '{user.Email.Value}' ya está en uso por otro usuario.");
                    }
                }
                _users[user.Id] = user; // Actualiza el objeto en el diccionario
            }
            else
            {
                throw new InvalidOperationException($"Usuario con ID {user.Id} no encontrado para actualizar.");
            }
            return Task.CompletedTask;
        }
    }
}