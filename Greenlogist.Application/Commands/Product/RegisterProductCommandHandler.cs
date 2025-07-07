using MediatR;
using Greenlogist.Domain.Aggregates.Product;
using Greenlogist.Domain.Repositories;
using Greenlogist.Domain.ValueObjects;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To check if ProducerId is a valid Producer
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Commands.Product
{
    /// <summary>
    /// Handler for the product registration command.
    /// </summary>
    public class RegisterProductCommandHandler : IRequestHandler<RegisterProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository; // To validate ProducerId

        public RegisterProductCommandHandler(IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(RegisterProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Validate if a product with the same name already exists for this producer
            if (await _productRepository.ExistsProductByNameAndProducerId(request.Name, request.ProducerId))
            {
                throw new ApplicationException($"A product with the name '{request.Name}' already exists for this producer.");
            }

            // 3. Create Value Objects
            var quantity = new Quantity(request.QuantityValue, request.QuantityUnit);
            var price = new Price(request.PriceValue, request.PriceCurrency);

            // 4. Create the 'Product' domain entity
            var product = new Product(Guid.NewGuid(), request.ProducerId, request.Name, request.Description, quantity, price);

            // 5. Persist the product
            await _productRepository.AddAsync(product);

            // 6. Return the ID of the new product
            return product.Id;
        }
    }
}