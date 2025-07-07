using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.Interfaces; // Para IPasswordHasher y IJwtTokenGenerator
using Greenlogist.Application.Common; // Para ApplicationException
using Greenlogist.Application.DTOs.Auth;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token

namespace Greenlogist.Application.Commands.Auth
{
    /// <summary>
    /// Manejador para el comando de inicio de sesión.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthTokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ApplicationException("Credenciales inválidas.");
            }

            // 2. Verificar la contraseña
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash.Value))
            {
                throw new ApplicationException("Credenciales inválidas.");
            }

            // 3. Generar token JWT
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email.Value, user.Role.ToString());

            return new AuthTokenResponse(token, user.Id, user.FullName, user.Email.Value, user.Role.ToString());
        }
    }
}
