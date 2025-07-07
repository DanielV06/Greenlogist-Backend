namespace Greenlogist.Application.DTOs.Auth
{
    /// <summary>
    /// DTO para la respuesta de inicio de sesión, conteniendo el token de autenticación.
    /// </summary>
    public record AuthTokenResponse(string Token, Guid UserId, string FullName, string Email, string Role);
}
