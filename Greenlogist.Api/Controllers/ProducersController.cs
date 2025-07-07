using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Greenlogist.Application.Commands.User;
using Greenlogist.Application.Queries.User;
using Greenlogist.Application.DTOs.User;
using System.Security.Claims;
using Greenlogist.Application.Common; // Para ApplicationException
using Swashbuckle.AspNetCore.Annotations; // Para Swagger
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Producer")] // Requiere autenticación y rol de Productor
    public class ProducersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProducersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene el perfil del productor autenticado.
        /// </summary>
        /// <returns>Los datos del perfil del productor.</returns>
        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Obtiene el perfil del productor", Description = "Requiere autenticación de productor. Devuelve los detalles del perfil del productor autenticado.")]
        [SwaggerResponse(200, "Perfil del productor obtenido exitosamente", typeof(ProducerProfileDto))]
        [SwaggerResponse(401, "No autorizado.")]
        [SwaggerResponse(403, "Acceso denegado (no es un productor).")]
        [SwaggerResponse(404, "Productor no encontrado.")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Obtener el ID del productor desde el token JWT
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "ID de usuario no encontrado en el token o formato inválido." });
                }

                var query = new GetProducerProfileQuery(producerId);
                var profile = await _mediator.Send(query);
                return Ok(profile);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "Ocurrió un error interno al obtener el perfil del productor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el perfil del productor autenticado.
        /// </summary>
        /// <param name="request">Datos para actualizar el perfil.</param>
        /// <returns>No content si la actualización es exitosa.</returns>
        [HttpPut("profile")]
        [SwaggerOperation(Summary = "Actualiza el perfil del productor", Description = "Requiere autenticación de productor. Permite actualizar el nombre, descripción, URL de imagen de perfil y opcionalmente la contraseña.")]
        [SwaggerResponse(204, "Perfil actualizado exitosamente.")]
        [SwaggerResponse(400, "Solicitud inválida.")]
        [SwaggerResponse(401, "No autorizado.")]
        [SwaggerResponse(403, "Acceso denegado (no es un productor).")]
        [SwaggerResponse(404, "Productor no encontrado.")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProducerProfileRequest request)
        {
            try
            {
                // Obtener el ID del productor desde el token JWT
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "ID de usuario no encontrado en el token o formato inválido." });
                }

                var command = new UpdateProducerProfileCommand(
                    producerId,
                    request.FullName,
                    request.Description,
                    request.ProfileImageUrl,
                    request.NewPassword
                );
                await _mediator.Send(command);
                return NoContent(); // 204 No Content
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "Ocurrió un error interno al actualizar el perfil del productor.", details = ex.Message });
            }
        }
    }
}