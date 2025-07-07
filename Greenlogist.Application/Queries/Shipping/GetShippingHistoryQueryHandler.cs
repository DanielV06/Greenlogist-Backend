using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Shipping;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Shipping
{
    /// <summary>
    /// Handler for the shipping history query.
    /// </summary>
    public class GetShippingHistoryQueryHandler : IRequestHandler<GetShippingHistoryQuery, IEnumerable<ShippingRequestDto>>
    {
        private readonly IShippingRequestRepository _shippingRequestRepository;
        private readonly IUserRepository _userRepository; // To validate ProducerId
        private readonly IProductRepository _productRepository; // To get product name for DTO

        public GetShippingHistoryQueryHandler(IShippingRequestRepository shippingRequestRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ShippingRequestDto>> Handle(GetShippingHistoryQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get shipping requests for the specified producer
            var shippingRequests = await _shippingRequestRepository.GetByProducerIdAsync(request.ProducerId);

            var dtos = new List<ShippingRequestDto>();
            foreach (var req in shippingRequests)
            {
                // For a real application, consider optimizing this loop to avoid N+1 queries.
                // A read model (CQRS query side) would typically pre-join this data.
                var product = await _productRepository.GetByIdAsync(req.ProductId);
                var productName = product?.Name ?? "Unknown Product";

                dtos.Add(new ShippingRequestDto(
                    req.Id,
                    req.ProductId,
                    productName,
                    req.Quantity.Value,
                    req.Quantity.Unit,
                    req.Origin.Address,
                    req.Destination.Address,
                    req.RequiredDate,
                    req.SpecialInstructions,
                    req.Status.ToString(),
                    req.CreatedAt
                ));
            }

            return dtos.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
