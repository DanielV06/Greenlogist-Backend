using MediatR;
using Greenlogist.Domain.Aggregates.Order;
using Greenlogist.Domain.Aggregates.Product; // To check product availability
using Greenlogist.Domain.Aggregates.User; // To validate ConsumerId and ProducerId
using Greenlogist.Domain.Repositories;
using Greenlogist.Domain.ValueObjects; // For Quantity and Price
using Greenlogist.Application.Common; // For ApplicationException
using System.Linq;
using System.Collections.Generic;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Commands.Order
{
    /// <summary>
    /// Handler for the place order command.
    /// </summary>
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public PlaceOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate if ConsumerId and ProducerId exist and have correct roles
            var consumer = await _userRepository.GetByIdAsync(request.ConsumerId);
            if (consumer == null || consumer.Role != UserRole.Consumer)
            {
                throw new ApplicationException("Invalid ConsumerId. Consumer not found or is not a consumer.");
            }

            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Validate order items and check product availability and price
            var orderItems = new List<OrderItem>();
            foreach (var itemRequest in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
                if (product == null || product.ProducerId != request.ProducerId)
                {
                    throw new ApplicationException($"Product with ID {itemRequest.ProductId} not found or does not belong to this producer.");
                }
                if (product.Quantity.Value < itemRequest.QuantityValue || product.Quantity.Unit != itemRequest.QuantityUnit)
                {
                    throw new ApplicationException($"Insufficient quantity of product '{product.Name}' available for order or unit mismatch.");
                }
                if (product.Price.Value != itemRequest.UnitPriceValue || product.Price.Currency != itemRequest.UnitPriceCurrency)
                {
                    // This is a critical business rule: ensure the price matches the current product price
                    throw new ApplicationException($"Price mismatch for product '{product.Name}'. Current price is {product.Price.Value} {product.Price.Currency}.");
                }

                // Reduce product quantity (cross-aggregate operation)
                // In a more robust system, this might be handled by an event listener for eventual consistency.
                product.ReduceQuantity(itemRequest.QuantityValue);
                await _productRepository.UpdateAsync(product);

                orderItems.Add(new OrderItem(
                    Guid.NewGuid(),
                    itemRequest.ProductId,
                    product.Name, // Use product's name
                    new Quantity(itemRequest.QuantityValue, itemRequest.QuantityUnit),
                    new Price(itemRequest.UnitPriceValue, itemRequest.UnitPriceCurrency)
                ));
            }

            // 3. Create the 'Order' domain entity
            var order = new Order(Guid.NewGuid(), request.ConsumerId, request.ProducerId, orderItems);

            // 4. Persist the order
            await _orderRepository.AddAsync(order);

            // 5. Return the ID of the new order
            return order.Id;
        }
    }
}