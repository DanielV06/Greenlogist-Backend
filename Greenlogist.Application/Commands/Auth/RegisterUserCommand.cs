using MediatR; // Usaremos MediatR para la implementación de CQRS

namespace Greenlogist.Application.Commands.Auth
{
    /// <summary>
    /// Comando para registrar un nuevo usuario.
    /// </summary>
    public record RegisterUserCommand(string FullName, string Email, string Password, string Role) : IRequest<Guid>;
}

