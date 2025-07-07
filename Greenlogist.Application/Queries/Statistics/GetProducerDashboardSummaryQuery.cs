using MediatR;
using Greenlogist.Application.DTOs.Statistics;

namespace Greenlogist.Application.Queries.Statistics
{
    /// <summary>
    /// Query to get a summary of statistics for a producer's dashboard.
    /// </summary>
    public record GetProducerDashboardSummaryQuery(Guid ProducerId) : IRequest<DashboardSummaryDto>;
}
