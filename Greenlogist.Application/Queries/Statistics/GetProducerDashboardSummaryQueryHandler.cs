using MediatR;
using Greenlogist.Domain.Repositories;
using Greenlogist.Application.DTOs.Statistics;
using Greenlogist.Application.Common; // For ApplicationException
using Greenlogist.Domain.Aggregates.User; // To validate ProducerId
using Greenlogist.Domain.Aggregates.Order; // For OrderStatus
using System.Linq;
using System.Threading.Tasks;
using Greenlogist.Backend.Greenlogist.Application.DTOs.Statistics;
using ApplicationException = Greenlogist.Application.Common.ApplicationException; // To get user ID from token


namespace Greenlogist.Application.Queries.Statistics
{
    /// <summary>
    /// Handler for the query to get a producer's dashboard summary.
    /// </summary>
    public class GetProducerDashboardSummaryQueryHandler : IRequestHandler<GetProducerDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IShippingRequestRepository _shippingRequestRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository; // To validate ProducerId

        public GetProducerDashboardSummaryQueryHandler(
            IProductRepository productRepository,
            IShippingRequestRepository shippingRequestRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<DashboardSummaryDto> Handle(GetProducerDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            // 1. Validate if the ProducerId exists and corresponds to a Producer
            var producer = await _userRepository.GetByIdAsync(request.ProducerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                throw new ApplicationException("Invalid ProducerId. Producer not found or is not a producer.");
            }

            // 2. Get data from various repositories
            var products = await _productRepository.GetProductsByProducerIdAsync(request.ProducerId);
            var shippingRequests = await _shippingRequestRepository.GetByProducerIdAsync(request.ProducerId);
            var salesOrders = await _orderRepository.GetOrdersByProducerIdAsync(request.ProducerId);

            // 3. Aggregate data for the summary
            var registeredProductsCount = products.Count();
            var requestedTransportsCount = shippingRequests.Count();
            var totalSalesAmount = salesOrders.Sum(o => o.TotalAmount);
            var totalProductsSoldKg = salesOrders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Quantity.Unit.Equals("kg", StringComparison.OrdinalIgnoreCase)) // Sum only 'kg' for now
                .Sum(oi => oi.Quantity.Value);

            // Placeholder for environmental impact - would require more complex calculation
            // For now, let's just return a dummy value or 0
            decimal environmentalImpactMetric = 0; // This would come from a dedicated calculation service

            // You might also calculate a percentage change for 'Estadísticas' if historical data is available
            // For now, we'll just return a dummy value or a simple count.
            // For the dashboard, the '+12%' is likely a trend, which requires more data points.
            // We'll return a simple count of completed orders as a proxy for "statistics" for now.
            var completedOrdersCount = salesOrders.Count(o => o.Status == OrderStatus.Completed);


            return new DashboardSummaryDto(
                registeredProductsCount,
                requestedTransportsCount,
                completedOrdersCount, // Using completed orders as a proxy for "statistics"
                totalSalesAmount,
                totalProductsSoldKg,
                environmentalImpactMetric
            );
        }
    }
}
