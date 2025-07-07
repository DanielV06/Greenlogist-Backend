using MediatR;
using Greenlogist.Application.DTOs.Order;
using System.Collections.Generic;

namespace Greenlogist.Application.Queries.Order
{
    /// <summary>
    /// Query to get all sales (orders) for a specific producer.
    /// </summary>
    public record GetProducerSalesQuery(Guid ProducerId) : IRequest<IEnumerable<ProducerSaleDto>>;
}
