using MediatR;
using Greenlogist.Application.DTOs.Shipping;
using System.Collections.Generic;

namespace Greenlogist.Application.Queries.Shipping
{
    /// <summary>
    /// Query to get the shipping history for a specific producer.
    /// </summary>
    public record GetShippingHistoryQuery(Guid ProducerId) : IRequest<IEnumerable<ShippingRequestDto>>;
}
