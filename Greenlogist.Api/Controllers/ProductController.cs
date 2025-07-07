using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Greenlogist.Application.Commands.Product;
using Greenlogist.Application.Queries.Product;
using Greenlogist.Application.DTOs.Product;
using Greenlogist.Application.Common; // For ApplicationException
using Swashbuckle.AspNetCore.Annotations; // For Swagger
using System.Security.Claims; // To get user ID from token
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Producer")] // Requires authentication and Producer role
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new product for the authenticated producer.
        /// </summary>
        /// <param name="request">Product registration data.</param>
        /// <returns>The ID of the newly registered product.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Registers a new product", Description = "Allows an authenticated producer to register a new organic product.")]
        [SwaggerResponse(201, "Product registered successfully", typeof(Guid))]
        [SwaggerResponse(400, "Invalid request or a product with the same name already exists for this producer.")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a producer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> RegisterProduct([FromBody] RegisterProductRequest request)
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var command = new RegisterProductCommand(
                    producerId,
                    request.Name,
                    request.Description,
                    request.QuantityValue,
                    request.QuantityUnit,
                    request.PriceValue,
                    request.PriceCurrency
                );
                var productId = await _mediator.Send(command);
                return StatusCode(201, productId); // 201 Created
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while registering the product.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets a list of products available for transport for the authenticated producer.
        /// </summary>
        /// <returns>A list of products with their ID, name, quantity, and unit.</returns>
        [HttpGet("for-transport")]
        [SwaggerOperation(Summary = "Gets products for transport", Description = "Retrieves a list of products owned by the authenticated producer that are available for transport requests.")]
        [SwaggerResponse(200, "Products retrieved successfully", typeof(IEnumerable<ProductForTransportDto>))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a producer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetProductsForTransport()
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetAvailableProductsForTransportQuery(producerId);
                var products = await _mediator.Send(query);
                return Ok(products);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message }); // e.g., if producer not found
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving products for transport.", details = ex.Message });
            }
        }
    }
}
