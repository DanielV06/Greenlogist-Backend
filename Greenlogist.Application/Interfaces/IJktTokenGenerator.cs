namespace Greenlogist.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de generación de tokens JWT.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, string role);
    }
}
