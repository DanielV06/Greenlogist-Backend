using MediatR;
using Greenlogist.Application.DTOs.User;

namespace Greenlogist.Application.Queries.User
{
    /// <summary>
    /// Query para obtener el perfil de un productor.
    /// </summary>
    public record GetProducerProfileQuery(Guid ProducerId) : IRequest<ProducerProfileDto>;
}
