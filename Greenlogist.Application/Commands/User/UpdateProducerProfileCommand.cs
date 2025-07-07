using MediatR;

namespace Greenlogist.Application.Commands.User
{
    /// <summary>
    /// Comando para actualizar el perfil de un productor.
    /// </summary>
    public record UpdateProducerProfileCommand(
        Guid ProducerId,
        string FullName,
        string? Description,
        string? ProfileImageUrl,
        string? NewPassword // Opcional, si se desea cambiar la contraseña
    ) : IRequest<Unit>; // Unit de MediatR indica que no hay valor de retorno
}
