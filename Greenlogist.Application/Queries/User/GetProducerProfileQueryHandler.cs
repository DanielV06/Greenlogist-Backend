using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.User;
using Greenlogist.Application.Common; // Para ApplicationException
using Greenlogist.Domain.Aggregates.User; // Para UserRole
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.User
{
    /// <summary>
    /// Manejador para la query de obtención del perfil de un productor.
    /// </summary>
    public class GetProducerProfileQueryHandler : IRequestHandler<GetProducerProfileQuery, ProducerProfileDto>
    {
        private readonly IUserRepository _userRepository;

        public GetProducerProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ProducerProfileDto> Handle(GetProducerProfileQuery request, CancellationToken cancellationToken)
        {
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);

            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Productor no encontrado o ID no corresponde a un productor.");
            }

            return new ProducerProfileDto(
                producer.Id,
                producer.FullName,
                producer.Email.Value,
                producer.Description,
                producer.ProfileImageUrl
            );
        }
    }
}
