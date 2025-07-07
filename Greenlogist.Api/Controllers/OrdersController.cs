using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Greenlogist.Application.Commands.Order;
using Greenlogist.Application.Queries.Order;
using Greenlogist.Application.DTOs.Order;
using Greenlogist.Application.Common; // For ApplicationException
using Swashbuckle.AspNetCore.Annotations; // For Swagger
using System.Security.Claims; // To get user ID and role from token
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all endpoints in this controller
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Places a new order for a consumer.
        /// </summary>
        /// <param name="request">Order placement data.</param>
        /// <returns>The ID of the newly placed order.</returns>
        [HttpPost]
        [Authorize(Roles = "Consumer")] // Only consumers can place orders
        [SwaggerOperation(Summary = "Places a new order", Description = "Allows an authenticated consumer to place a new order for products from a specific producer.")]
        [SwaggerResponse(201, "Order placed successfully", typeof(Guid))]
        [SwaggerResponse(400, "Invalid request (e.g., product not found, insufficient quantity, price mismatch).")]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a consumer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                // Get consumer ID from JWT token (ensure it matches request.ConsumerId for security)
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid consumerIdFromToken))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                // Security check: Ensure the consumer placing the order is the authenticated user
                if (consumerIdFromToken != request.ConsumerId)
                {
                    return Forbid(new { message = "You can only place orders for your own consumer ID." });
                }

                var command = new PlaceOrderCommand(
                    request.ConsumerId,
                    request.ProducerId,
                    request.Items
                );
                var orderId = await _mediator.Send(command);
                return StatusCode(201, orderId); // 201 Created
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while placing the order.", details = ex.Message });
            }
        }

        private IActionResult Forbid(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the order history for the authenticated consumer.
        /// </summary>
        /// <returns>A list of orders made by the consumer.</returns>
        [HttpGet("consumer-history")]
        [Authorize(Roles = "Consumer")] // Only consumers can view their order history
        [SwaggerOperation(Summary = "Gets consumer's order history", Description = "Retrieves all orders placed by the authenticated consumer.")]
        [SwaggerResponse(200, "Order history retrieved successfully", typeof(IEnumerable<OrderDto>))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a consumer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetConsumerOrderHistory()
        {
            try
            {
                // Get consumer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid consumerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetConsumerOrdersQuery(consumerId);
                var history = await _mediator.Send(query);
                return Ok(history);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving consumer order history.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets the sales history for the authenticated producer.
        /// </summary>
        /// <returns>A list of sales (orders) received by the producer.</returns>
        [HttpGet("producer-sales")]
        [Authorize(Roles = "Producer")] // Only producers can view their sales history
        [SwaggerOperation(Summary = "Gets producer's sales history", Description = "Retrieves all orders received by the authenticated producer.")]
        [SwaggerResponse(200, "Sales history retrieved successfully", typeof(IEnumerable<ProducerSaleDto>))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a producer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetProducerSalesHistory()
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetProducerSalesQuery(producerId);
                var sales = await _mediator.Send(query);
                return Ok(sales);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving producer sales history.", details = ex.Message });
            }
        }
    }
}
