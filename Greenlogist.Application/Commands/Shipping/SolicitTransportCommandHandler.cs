using MediatR;
using Greenlogist.Domain.Aggregates.Shipping;
using Greenlogist.Domain.Aggregates.Product; // To check product availability
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using Greenlogist.Domain.Repositories;
using Greenlogist.Domain.ValueObjects;
using Greenlogist.Application.Common; // For ApplicationException
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Commands.Shipping
{
    /// <summary>
    /// Handler for the solicit transport command.
    /// </summary>
    public class SolicitTransportCommandHandler : IRequestHandler<SolicitTransportCommand, Guid>
    {
        private readonly IShippingRequestRepository _shippingRequestRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public SolicitTransportCommandHandler(
            IShippingRequestRepository shippingRequestRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(SolicitTransportCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get the product and check availability
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null || product.ProducerId != request.ProducerId)
            {
                throw new ApplicationException("Product not found or does not belong to this producer.");
            }
            if (product.Quantity.Value < request.QuantityValue || product.Quantity.Unit != request.QuantityUnit)
            {
                throw new ApplicationException($"Insufficient quantity of product '{product.Name}' available for transport or unit mismatch.");
            }

            // 3. Create Value Objects
            var quantity = new Quantity(request.QuantityValue, request.QuantityUnit);
            var originLocation = new Location(request.OriginAddress, request.OriginCity, request.OriginCountry);
            var destinationLocation = new Location(request.DestinationAddress, request.DestinationCity, request.DestinationCountry);

            // 4. Create the 'ShippingRequest' domain entity
            var shippingRequest = new ShippingRequest(
                Guid.NewGuid(),
                request.ProducerId,
                request.ProductId,
                quantity,
                originLocation,
                destinationLocation,
                request.RequiredDate,
                request.SpecialInstructions
            );

            // 5. Reduce product quantity (This is a cross-aggregate operation, consider domain events for eventual consistency)
            // For simplicity, we're doing it directly here. In a more complex scenario,
            // 'TransportRequestedEvent' would be published, and a handler in the Product context
            // would listen and reduce the quantity.
            product.ReduceQuantity(request.QuantityValue);
            await _productRepository.UpdateAsync(product);

            // 6. Persist the shipping request
            await _shippingRequestRepository.AddAsync(shippingRequest);

            // 7. Return the ID of the new shipping request
            return shippingRequest.Id;
        }
    }
}
