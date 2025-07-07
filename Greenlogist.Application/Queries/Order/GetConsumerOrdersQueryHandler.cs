using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Order;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ConsumerId
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Order
{
    /// <summary>
    /// Handler for the query to get consumer orders.
    /// </summary>
    public class GetConsumerOrdersQueryHandler : IRequestHandler<GetConsumerOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public GetConsumerOrdersQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetConsumerOrdersQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ConsumerId exists and corresponds to a Consumer
            var consumer = await _userRepository.GetByIdAsync(request.ConsumerId);
            if (consumer == null || consumer.Role != UserRole.Consumer)
            {
                throw new ApplicationException("Invalid ConsumerId. Consumer not found or is not a consumer.");
            }

            // 2. Get orders for the specified consumer
            var orders = await _orderRepository.GetOrdersByConsumerIdAsync(request.ConsumerId);

            // 3. Map domain orders to DTOs
            return orders.Select(o => new OrderDto(
                o.Id,
                o.ConsumerId,
                o.ProducerId,
                o.OrderDate,
                o.Status.ToString(),
                o.TotalAmount,
                o.OrderItems.Select(oi => new OrderItemDto(
                    oi.ProductId,
                    oi.ProductName,
                    oi.Quantity.Value,
                    oi.Quantity.Unit,
                    oi.UnitPrice.Value,
                    oi.UnitPrice.Currency
                )).ToList()
            )).OrderByDescending(o => o.OrderDate).ToList();
        }
    }
}