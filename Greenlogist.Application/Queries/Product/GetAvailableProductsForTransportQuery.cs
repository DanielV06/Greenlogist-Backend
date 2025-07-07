using MediatR;
using Greenlogist.Application.DTOs.Product;
using System.Collections.Generic;

namespace Greenlogist.Application.Queries.Product
{
    /// <summary>
    /// Query to get available products for transport for a specific producer.
    /// </summary>
    public record GetAvailableProductsForTransportQuery(Guid ProducerId) : IRequest<IEnumerable<ProductForTransportDto>>;
}