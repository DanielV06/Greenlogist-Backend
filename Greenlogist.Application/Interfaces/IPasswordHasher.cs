namespace Greenlogist.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de hashing de contraseñas.
    /// </summary>
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}