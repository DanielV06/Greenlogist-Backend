using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Statistics;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using Greenlogist.Domain.Aggregates.Order; // For OrderStatus
using System.Linq;
using System.Threading.Tasks;
using Greenlogist.Backend.Greenlogist.Application.DTOs.Statistics;
using Greenlogist.Domain.Aggregates.Shipping;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Statistics
{
    /// <summary>
    /// Handler for the query to get detailed producer statistics.
    /// </summary>
    public class GetProducerStatisticsQueryHandler : IRequestHandler<GetProducerStatisticsQuery, ProducerStatisticsDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingRequestRepository _shippingRequestRepository;
        private readonly IUserRepository _userRepository; // To validate ProducerId

        public GetProducerStatisticsQueryHandler(
            IOrderRepository orderRepository,
            IShippingRequestRepository shippingRequestRepository,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _userRepository = userRepository;
        }

        public async Task<ProducerStatisticsDto> Handle(GetProducerStatisticsQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get data from repositories
            var salesOrders = await _orderRepository.GetOrdersByProducerIdAsync(request.ProducerId);
            var shippingRequests = await _shippingRequestRepository.GetByProducerIdAsync(request.ProducerId);

            // 3. Aggregate data for detailed statistics
            var totalSalesCount = salesOrders.Count();
            var totalSalesAmount = salesOrders.Sum(o => o.TotalAmount);
            var totalProductsSoldKg = salesOrders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Quantity.Unit.Equals("kg", StringComparison.OrdinalIgnoreCase))
                .Sum(oi => oi.Quantity.Value);

            var totalTransportRequests = shippingRequests.Count();
            var completedTransportRequests = shippingRequests.Count(sr => sr.Status == ShippingStatus.Completed);

            // Placeholder for environmental impact - would require a dedicated calculation
            decimal totalEnvironmentalImpact = 0; // This would come from a dedicated calculation service

            return new ProducerStatisticsDto(
                totalSalesCount,
                totalSalesAmount,
                totalProductsSoldKg,
                totalTransportRequests,
                completedTransportRequests,
                totalEnvironmentalImpact
            );
        }
    }
}
