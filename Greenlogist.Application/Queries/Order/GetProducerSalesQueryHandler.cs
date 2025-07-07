using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Order;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Order
{
    /// <summary>
    /// Handler for the query to get producer sales.
    /// </summary>
    public class GetProducerSalesQueryHandler : IRequestHandler<GetProducerSalesQuery, IEnumerable<ProducerSaleDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public GetProducerSalesQueryHandler(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ProducerSaleDto>> Handle(GetProducerSalesQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get orders for the specified producer
            var salesOrders = await _orderRepository.GetOrdersByProducerIdAsync(request.ProducerId);

            // 3. Map domain orders to DTOs for producer sales
            return salesOrders.Select(o => new ProducerSaleDto(
                o.Id,
                o.ConsumerId,
                o.OrderDate,
                o.Status.ToString(),
                o.TotalAmount,
                o.OrderItems.Select(oi => new OrderItemDto( // Reusing OrderItemDto for simplicity
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
