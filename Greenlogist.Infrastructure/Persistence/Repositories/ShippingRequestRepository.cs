using Greenlogist.Domain.Aggregates.Shipping;
using Greenlogist.Domain.Repositories;
using System.Collections.Concurrent; // To simulate an in-memory database
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Greenlogist.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of IShippingRequestRepository that simulates an in-memory database.
    /// In a real application, this would interact with EF Core, Dapper, etc.
    /// </summary>
    public class ShippingRequestRepository : IShippingRequestRepository
    {
        // In-memory database simulation
        private static readonly ConcurrentDictionary<Guid, ShippingRequest> _shippingRequests = new ConcurrentDictionary<Guid, ShippingRequest>();

        public Task AddAsync(ShippingRequest request)
        {
            if (!_shippingRequests.TryAdd(request.Id, request))
            {
                throw new InvalidOperationException($"Could not add shipping request with ID {request.Id}. It already exists.");
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ShippingRequest request)
        {
            _shippingRequests.TryRemove(request.Id, out _);
            return Task.CompletedTask;
        }

        public Task<ShippingRequest?> GetByIdAsync(Guid id)
        {
            _shippingRequests.TryGetValue(id, out ShippingRequest? request);
            return Task.FromResult(request);
        }

        public Task<IEnumerable<ShippingRequest>> GetByProducerIdAsync(Guid producerId)
        {
            var producerRequests = _shippingRequests.Values.Where(r => r.ProducerId == producerId).ToList();
            return Task.FromResult<IEnumerable<ShippingRequest>>(producerRequests);
        }

        public Task<IEnumerable<ShippingRequest>> GetPendingRequestsAsync()
        {
            var pendingRequests = _shippingRequests.Values.Where(r => r.Status == ShippingStatus.Pending).ToList();
            return Task.FromResult<IEnumerable<ShippingRequest>>(pendingRequests);
        }

        public Task UpdateAsync(ShippingRequest request)
        {
            if (_shippingRequests.ContainsKey(request.Id))
            {
                _shippingRequests[request.Id] = request; // Update the object in the dictionary
            }
            else
            {
                throw new InvalidOperationException($"Shipping request with ID {request.Id} not found for update.");
            }
            return Task.CompletedTask;
        }
    }
}