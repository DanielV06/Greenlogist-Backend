using MediatR;
using Greenlogist.Domain.Aggregates.User;
using Greenlogist.Domain.Repositories;
using Greenlogist.Domain.ValueObjects;
using Greenlogist.Application.Interfaces; // Para IPasswordHasher
using Greenlogist.Application.Common; // Para ApplicationException
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Commands.Auth
{
    /// <summary>
    /// Manejador para el comando de registro de usuario.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar si el email ya existe
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new ApplicationException($"El correo electrónico '{request.Email}' ya está registrado.");
            }

            // 2. Hashear la contraseña
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 3. Crear objetos de valor y validar el rol
            Email userEmail = new Email(request.Email);
            PasswordHash userPasswordHash = new PasswordHash(passwordHash);

            if (!Enum.TryParse(request.Role, true, out UserRole userRole))
            {
                throw new ApplicationException($"El rol '{request.Role}' no es válido.");
            }

            // 4. Crear la entidad de dominio 'User'
            var user = new Greenlogist.Domain.Aggregates.User.User(Guid.NewGuid(), request.FullName, userEmail, userPasswordHash, userRole);

            // 5. Persistir el usuario
            await _userRepository.AddAsync(user);

            // 6. Devolver el ID del nuevo usuario
            return user.Id;
        }
    }
}