using MediatR;
using Greenlogist.Application.DTOs.Statistics;

namespace Greenlogist.Application.Queries.Statistics
{
    /// <summary>
    /// Query to get detailed statistics for a producer.
    /// </summary>
    public record GetProducerStatisticsQuery(Guid ProducerId) : IRequest<ProducerStatisticsDto>;
}
