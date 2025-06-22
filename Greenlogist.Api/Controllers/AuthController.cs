using Greenlogist.Api.DTOs;
using Greenlogist.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Greenlogist.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Esto se traduce en la ruta: /api/auth
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // --- Endpoint de Registro (ya existente) ---
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request.FullName, request.Email, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return BadRequest(ModelState);
        }

        return Ok(new { Message = "User registered successfully" });
    }
    
    // --- NUEVO ENDPOINT DE LOGIN ---
    [HttpPost("login")] // Endpoint: POST /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Succeeded)
        {
            // Para login, es mejor devolver un error 401 Unauthorized si las credenciales son inválidas.
            return Unauthorized(new { Message = "Invalid credentials." });
        }
        
        // Si el login es exitoso, devolvemos el token.
        return Ok(new { Token = result.Token });
    }
}