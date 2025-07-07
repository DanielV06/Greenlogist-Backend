using Greenlogist.Domain.Aggregates.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greenlogist.Domain.Repositories
{
    /// <summary>
    /// Repository interface for the 'ShippingRequest' aggregate.
    /// Defines contracts for shipping request persistence and retrieval.
    /// </summary>
    public interface IShippingRequestRepository
    {
        Task<ShippingRequest?> GetByIdAsync(Guid id);
        Task AddAsync(ShippingRequest request);
        Task UpdateAsync(ShippingRequest request);
        Task DeleteAsync(ShippingRequest request);
        Task<IEnumerable<ShippingRequest>> GetByProducerIdAsync(Guid producerId); // Get all shipping requests for a producer
        Task<IEnumerable<ShippingRequest>> GetPendingRequestsAsync(); // Get all pending requests (e.g., for a logistics service)
    }
}
