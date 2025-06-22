namespace Greenlogist.Application.Interfaces;

// Usaremos un DTO para la respuesta también, lo crearemos en el siguiente paso.
// Por ahora, solo necesitamos una referencia a un tipo de resultado que definiremos.
public record AuthResult(bool Succeeded, string[] Errors);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string fullName, string email, string password);
}