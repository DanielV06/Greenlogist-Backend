using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Greenlogist.Application.Queries.Statistics;
using Greenlogist.Application.DTOs.Statistics;
using Greenlogist.Application.Common; // For ApplicationException
using Swashbuckle.AspNetCore.Annotations; // For Swagger
using System.Security.Claims; // To get user ID from token
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Producer")] // Requires authentication and Producer role
    public class StatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a summary of key statistics for the authenticated producer's dashboard.
        /// </summary>
        /// <returns>A summary of products registered, transports requested, and sales overview.</returns>
        [HttpGet("dashboard-summary")]
        [SwaggerOperation(Summary = "Gets producer dashboard summary", Description = "Retrieves a high-level summary of products, transports, and sales for the authenticated producer's dashboard.")]
        [SwaggerResponse(200, "Dashboard summary retrieved successfully", typeof(DashboardSummaryDto))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a producer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetProducerDashboardSummaryQuery(producerId);
                var summary = await _mediator.Send(query);
                return Ok(summary);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving dashboard summary.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gets detailed statistics for the authenticated producer.
        /// </summary>
        /// <returns>Detailed statistics including sales, products sold, and transport metrics.</returns>
        [HttpGet("detailed-statistics")]
        [SwaggerOperation(Summary = "Gets detailed producer statistics", Description = "Retrieves comprehensive statistics for the authenticated producer, including sales, products sold, and transport details.")]
        [SwaggerResponse(200, "Detailed statistics retrieved successfully", typeof(ProducerStatisticsDto))]
        [SwaggerResponse(401, "Unauthorized.")]
        [SwaggerResponse(403, "Access denied (not a producer).")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetDetailedStatistics()
        {
            try
            {
                // Get producer ID from JWT token
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid producerId))
                {
                    return Unauthorized(new { message = "User ID not found in token or invalid format." });
                }

                var query = new GetProducerStatisticsQuery(producerId);
                var statistics = await _mediator.Send(query);
                return Ok(statistics);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An internal error occurred while retrieving detailed statistics.", details = ex.Message });
            }
        }
    }
}
