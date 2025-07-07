using MediatR;
using Greenlogist.Application.DTOs.Product;
using System.Collections.Generic;
using Greenlogist.Backend.Greenlogist.Application.DTOs.Product;

namespace Greenlogist.Application.Queries.Product
{
    /// <summary>
    /// Query to get available products for transport for a specific producer.
    /// </summary>
    public record GetAvailableProductsForTransportQuery(Guid ProducerId) : IRequest<IEnumerable<ProductForTransportDto>>;
}