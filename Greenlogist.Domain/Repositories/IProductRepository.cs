using Greenlogist.Domain.Aggregates.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greenlogist.Domain.Repositories
{
    /// <summary>
    /// Repository interface for the 'Product' aggregate.
    /// Defines contracts for product persistence and retrieval.
    /// </summary>
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<IEnumerable<Product>> GetProductsByProducerIdAsync(Guid producerId); // To get products for a specific producer
        Task<bool> ExistsProductByNameAndProducerId(string productName, Guid producerId); // To prevent duplicate product names for a producer
    }
}