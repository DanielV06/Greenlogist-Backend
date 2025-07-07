using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Product;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenlogist.Backend.Greenlogist.Application.DTOs.Product;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Product
{
    /// <summary>
    /// Handler for the query to get available products for transport.
    /// </summary>
    public class GetAvailableProductsForTransportQueryHandler : IRequestHandler<GetAvailableProductsForTransportQuery, IEnumerable<ProductForTransportDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository; // To validate ProducerId

        public GetAvailableProductsForTransportQueryHandler(IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ProductForTransportDto>> Handle(GetAvailableProductsForTransportQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get products for the specified producer
            var products = await _productRepository.GetProductsByProducerIdAsync(request.ProducerId);

            // 3. Map domain products to DTOs
            return products.Select(p => new ProductForTransportDto(
                p.Id,
                p.Name,
                p.Quantity.Value,
                p.Quantity.Unit
            )).ToList();
        }
    }
}