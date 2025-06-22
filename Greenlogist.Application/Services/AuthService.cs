using Greenlogist.Application.Interfaces;
using Greenlogist.Domain;
using Microsoft.AspNetCore.Identity;

namespace Greenlogist.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResult> RegisterAsync(string fullName, string email, string password)
    {
        // 1. Verificar si el usuario ya existe
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new AuthResult(false, new[] { "A user with this email already exists." });
        }

        // 2. Crear el objeto del nuevo usuario
        var newUser = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email // Por defecto, usamos el email como nombre de usuario
        };

        // 3. Usar UserManager para crear el usuario con la contraseña
        //    Esto se encarga automáticamente de hashear la contraseña
        var result = await _userManager.CreateAsync(newUser, password);

        // 4. Verificar el resultado y devolver nuestro AuthResult personalizado
        if (!result.Succeeded)
        {
            // Convertir los errores de Identity a un array de strings
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return new AuthResult(false, errors);
        }

        return new AuthResult(true, Array.Empty<string>());
    }
}