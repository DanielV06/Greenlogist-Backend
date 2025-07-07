using Greenlogist.Domain.Aggregates.User;
using System.Threading.Tasks;

namespace Greenlogist.Domain.Repositories
{
    /// <summary>
    /// Interfaz de repositorio para la entidad 'User'.
    /// Define los contratos para la persistencia de usuarios.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> ExistsByEmailAsync(string email); // Para validar unicidad del email
    }
}
