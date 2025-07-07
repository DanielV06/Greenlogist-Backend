using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;
using Greenlogist.Domain.ValueObjects; // For Quantity and Price

namespace Greenlogist.Domain.Aggregates.Order
{
    /// <summary>
    /// Represents an item within an 'Order'. This is an entity within the Order aggregate.
    /// </summary>
    public class OrderItem : Entity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } // Denormalized for convenience in the order context
        public Quantity Quantity { get; private set; } // Quantity of the product in this order item
        public Price UnitPrice { get; private set; } // Price per unit at the time of purchase

        // Private constructor for reconstruction from persistence (DDD)
        private OrderItem() { }

        /// <summary>
        /// Constructor for creating a new order item.
        /// </summary>
        /// <param name="id">Unique identifier for the order item.</param>
        /// <param name="productId">ID of the product being ordered.</param>
        /// <param name="productName">Name of the product (denormalized).</param>
        /// <param name="quantity">Quantity of the product (as a value object).</param>
        /// <param name="unitPrice">Unit price of the product (as a value object).</param>
        public OrderItem(Guid id, Guid productId, string productName, Quantity quantity, Price unitPrice) : base(id)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Product ID cannot be empty for an order item.");
            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name cannot be empty for an order item.");
            if (quantity == null)
                throw new DomainException("Quantity cannot be null for an order item.");
            if (unitPrice == null)
                throw new DomainException("Unit price cannot be null for an order item.");

            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        // No methods to change ProductId, ProductName, UnitPrice, as OrderItem should be immutable after creation
        // Quantity could be updated if order modification is allowed, but for simplicity, we assume it's set at creation.
    }
}
