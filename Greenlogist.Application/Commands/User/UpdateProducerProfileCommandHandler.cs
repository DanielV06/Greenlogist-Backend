using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.Common; // Para ApplicationException
using Greenlogist.Domain.Aggregates.User;
using Greenlogist.Domain.ValueObjects;
using Greenlogist.Application.Interfaces; // Para IPasswordHasher
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Commands.User
{
    /// <summary>
    /// Manejador para el comando de actualización del perfil del productor.
    /// </summary>
    public class UpdateProducerProfileCommandHandler : IRequestHandler<UpdateProducerProfileCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateProducerProfileCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(UpdateProducerProfileCommand request, CancellationToken cancellationToken)
        {
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);

            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Productor no encontrado.");
            }

            // Actualizar información del perfil
            producer.UpdateProfile(request.FullName, request.Description, request.ProfileImageUrl);

            // Si se proporciona una nueva contraseña, actualizarla
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
                producer.ChangePassword(new PasswordHash(newPasswordHash));
            }

            await _userRepository.UpdateAsync(producer);

            return Unit.Value; // Indica que el comando se completó sin devolver un valor específico
        }
    }
}
