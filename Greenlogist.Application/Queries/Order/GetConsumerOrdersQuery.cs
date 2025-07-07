using MediatR;
using Greenlogist.Application.DTOs.Order;
using System.Collections.Generic;

namespace Greenlogist.Application.Queries.Order
{
    /// <summary>
    /// Query to get all orders for a specific consumer.
    /// </summary>
    public record GetConsumerOrdersQuery(Guid ConsumerId) : IRequest<IEnumerable<OrderDto>>;
}
