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

    [HttpPost("register")] // Endpoint: POST /api/auth/register
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // La validación de [Required] y [EmailAddress] del DTO ocurre automáticamente
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request.FullName, request.Email, request.Password);

        if (!result.Succeeded)
        {
            // Añadimos los errores que vienen de nuestro servicio para que el frontend los vea
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return BadRequest(ModelState);
        }

        return Ok(new { Message = "User registered successfully" });
    }
}