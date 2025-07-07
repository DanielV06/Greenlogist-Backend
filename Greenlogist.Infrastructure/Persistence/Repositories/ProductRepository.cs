using Greenlogist.Domain.Aggregates.Product;
using Greenlogist.Domain.Repositories;
using System.Collections.Concurrent; // To simulate an in-memory database
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Greenlogist.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementation of IProductRepository that simulates an in-memory database.
    /// In a real application, this would interact with EF Core, Dapper, etc.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        // In-memory database simulation
        private static readonly ConcurrentDictionary<Guid, Product> _products = new ConcurrentDictionary<Guid, Product>();

        public Task AddAsync(Product product)
        {
            if (!_products.TryAdd(product.Id, product))
            {
                throw new InvalidOperationException($"Could not add product with ID {product.Id}. It already exists.");
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _products.TryRemove(product.Id, out _);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            _products.TryGetValue(id, out Product? product);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetProductsByProducerIdAsync(Guid producerId)
        {
            var producerProducts = _products.Values.Where(p => p.ProducerId == producerId).ToList();
            return Task.FromResult<IEnumerable<Product>>(producerProducts);
        }

        public Task UpdateAsync(Product product)
        {
            if (_products.ContainsKey(product.Id))
            {
                _products[product.Id] = product; // Update the object in the dictionary
            }
            else
            {
                throw new InvalidOperationException($"Product with ID {product.Id} not found for update.");
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsProductByNameAndProducerId(string productName, Guid producerId)
        {
            return Task.FromResult(_products.Values.Any(p =>
                p.ProducerId == producerId &&
                p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}