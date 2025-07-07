using MediatR;
using Greenlogist.Application.DTOs.Auth; // Para AuthTokenResponse

namespace Greenlogist.Application.Commands.Auth
{
    /// <summary>
    /// Comando para iniciar sesión de un usuario.
    /// </summary>
    public record LoginCommand(string Email, string Password) : IRequest<AuthTokenResponse>;
}
