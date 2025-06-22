namespace Greenlogist.Application.Interfaces;

// Resultado para el registro
public record AuthResult(bool Succeeded, string[] Errors);

// Resultado para el login, que incluirá el token
public record LoginResult(bool Succeeded, string Token, string[] Errors);


public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string fullName, string email, string password);
    Task<LoginResult> LoginAsync(string email, string password);
}