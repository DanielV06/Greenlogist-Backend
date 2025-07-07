using Greenlogist.Domain.Aggregates.Order;
using Greenlogist.Domain.Repositories;
using System.Collections.Concurrent; // To simulate an in-memory database
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Greenlogist.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of IOrderRepository that simulates an in-memory database.
    /// In a real application, this would interact with EF Core, Dapper, etc.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        // In-memory database simulation
        private static readonly ConcurrentDictionary<Guid, Order> _orders = new ConcurrentDictionary<Guid, Order>();

        public Task AddAsync(Order order)
        {
            if (!_orders.TryAdd(order.Id, order))
            {
                throw new InvalidOperationException($"Could not add order with ID {order.Id}. It already exists.");
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order order)
        {
            _orders.TryRemove(order.Id, out _);
            return Task.CompletedTask;
        }

        public Task<Order?> GetByIdAsync(Guid id)
        {
            _orders.TryGetValue(id, out Order? order);
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetOrdersByConsumerIdAsync(Guid consumerId)
        {
            var consumerOrders = _orders.Values.Where(o => o.ConsumerId == consumerId).ToList();
            return Task.FromResult<IEnumerable<Order>>(consumerOrders);
        }

        public Task<IEnumerable<Order>> GetOrdersByProducerIdAsync(Guid producerId)
        {
            var producerSales = _orders.Values.Where(o => o.ProducerId == producerId).ToList();
            return Task.FromResult<IEnumerable<Order>>(producerSales);
        }

        public Task UpdateAsync(Order order)
        {
            if (_orders.ContainsKey(order.Id))
            {
                _orders[order.Id] = order; // Update the object in the dictionary
            }
            else
            {
                throw new InvalidOperationException($"Order with ID {order.Id} not found for update.");
            }
            return Task.CompletedTask;
        }
    }
}
