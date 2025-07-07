using Greenlogist.Domain.Aggregates.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greenlogist.Domain.Repositories
{
    /// <summary>
    /// Repository interface for the 'Order' aggregate.
    /// Defines contracts for order persistence and retrieval.
    /// </summary>
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order); // Consider if orders should be deletable or only cancelled
        Task<IEnumerable<Order>> GetOrdersByConsumerIdAsync(Guid consumerId);
        Task<IEnumerable<Order>> GetOrdersByProducerIdAsync(Guid producerId); // For producer's sales statistics
    }
}
