using Greenlogist.Application.Interfaces;
using Greenlogist.Domain;
using Microsoft.AspNetCore.Identity;

namespace Greenlogist.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // Inyectamos los servicios que necesitamos
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // --- El método de Registro ---
    public async Task<AuthResult> RegisterAsync(string fullName, string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new AuthResult(false, new[] { "A user with this email already exists." });
        }

        var newUser = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return new AuthResult(false, errors);
        }

        return new AuthResult(true, Array.Empty<string>());
    }
    
    // --- NUEVO MÉTODO DE LOGIN ---
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        // 1. Buscar al usuario por su email
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new LoginResult(false, "", new[] { "Invalid credentials." });
        }

        // 2. Usar SignInManager para verificar la contraseña de forma segura
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            return new LoginResult(false, "", new[] { "Invalid credentials." });
        }

        // 3. Si las credenciales son correctas, generar el token
        var token = _jwtTokenGenerator.GenerateToken(user);
        
        return new LoginResult(true, token, Array.Empty<string>());
    }
}