using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Greenlogist.Application.Commands.Shipping;
using Greenlogist.Application.Queries.Shipping;
using Greenlogist.Application.DTOs.Shipping;
using Greenlogist.Application.Common; // For ApplicationException
using Swashbuckle.AspNetCore.Annotations; // For Swagger
using System.Security.Claims;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token

namespace Greenlogist.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Producer")] // Requires authentication and Producer role
    public class ShippingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShippingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Solicits a new transport request for the authenticated producer.
        /// </summary>
        /// <param name="request">Transport request data.</param>
        /// <returns>The ID of the newly created shipping request.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Solicita un nuevo transporte", Description = "Permite a un productor autenticado solicitar el transporte de un producto.")]
        [SwaggerResponse(201, "Solicitud de transporte creada exitosamente", typeof(Guid))]
        [SwaggerResponse(400, "Solicitud inválida (ej. producto no encontrado, cantidad insuficiente, fecha inválida).")]
        [SwaggerResponse(401, "No autorizado.")]
        [SwaggerResponse(403, "Acceso denegado (no es un productor).")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> SolicitTransport([FromBody] RequestTransportRequest request)
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var command = new SolicitTransportCommand(
                    producerId,
                    request.ProductId,
                    request.QuantityValue,
                    request.QuantityUnit,
                    request.OriginAddress,
                    request.OriginCity,
                    request.OriginCountry,
                    request.DestinationAddress,
                    request.DestinationCity,
                    request.DestinationCountry,
                    request.RequiredDate,
                    request.SpecialInstructions
                );
                var shippingRequestId = await _mediator.Send(command);
                return StatusCode(201, shippingRequestId); // 201 Created
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while soliciting transport.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets the shipping history for the authenticated producer.
        /// </summary>
        /// <returns>A list of shipping requests made by the producer.</returns>
        [HttpGet("history")]
        [SwaggerOperation(Summary = "Obtiene el historial de envíos", Description = "Recupera todas las solicitudes de transporte realizadas por el productor autenticado.")]
        [SwaggerResponse(200, "Historial de envíos obtenido exitosamente", typeof(IEnumerable<ShippingRequestDto>))]
        [SwaggerResponse(401, "No autorizado.")]
        [SwaggerResponse(403, "Acceso denegado (no es un productor).")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> GetShippingHistory()
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetShippingHistoryQuery(producerId);
                var history = await _mediator.Send(query);
                return Ok(history);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message }); // e.g., if producer not found
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving shipping history.", details = ex.Message });
            }
        }
    }
}
