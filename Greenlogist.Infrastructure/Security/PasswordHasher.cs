using Greenlogist.Application.Interfaces;
using BCrypt.Net; // Instalar el paquete NuGet BCrypt.Net-Next

namespace Greenlogist.Infrastructure.Security
{
    /// <summary>
    /// Implementación de IPasswordHasher usando BCrypt para hashing y verificación de contraseñas.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Genera un salt y hashea la contraseña
            // BCrypt.Net-Next maneja el salt internamente
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Verifica si la contraseña en texto plano coincide con el hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}