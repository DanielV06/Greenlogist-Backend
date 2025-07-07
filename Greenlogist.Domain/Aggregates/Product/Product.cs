using Greenlogist.Backend.Greenlogist.Domain.Aggregates.Product;
using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;
using Greenlogist.Domain.ValueObjects; // For Price and Quantity

namespace Greenlogist.Domain.Aggregates.Product
{
    /// <summary>
    /// Represents the 'Product' aggregate root entity.
    /// </summary>
    public class Product : Entity, IAggregateRoot
    {
        public Guid ProducerId { get; private set; } // The ID of the producer who owns this product
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ValueObjects.Quantity Quantity { get; private set; } // Value object for quantity and unit
        public ValueObjects.Price Price { get; private set; } // Value object for price

        // Private constructor for reconstruction from persistence (DDD)
        private Product() { }

        /// <summary>
        /// Constructor for creating a new product.
        /// </summary>
        /// <param name="id">Unique identifier for the product.</param>
        /// <param name="producerId">ID of the producer who owns this product.</param>
        /// <param name="name">Name of the product.</param>
        /// <param name="description">Description of the product.</param>
        /// <param name="quantity">Quantity and unit of the product (as a value object).</param>
        /// <param name="price">Price of the product (as a value object).</param>
        public Product(Guid id, Guid producerId, string name, string description, ValueObjects.Quantity quantity, ValueObjects.Price price) : base(id)
        {
            if (producerId == Guid.Empty)
                throw new DomainException("Producer ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Product description cannot be empty.");
            if (quantity == null)
                throw new DomainException("Product quantity cannot be null.");
            if (price == null)
                throw new DomainException("Product price cannot be null.");

            ProducerId = producerId;
            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;

            // You could add a domain event here, e.g.:
            // AddDomainEvent(new ProductRegisteredEvent(id, producerId, name, quantity.Value, quantity.Unit, price.Value));
        }

        /// <summary>
        /// Updates the product's details.
        /// </summary>
        /// <param name="name">New product name.</param>
        /// <param name="description">New product description.</param>
        /// <param name="quantity">New quantity and unit (as a value object).</param>
        /// <param name="price">New price (as a value object).</param>
        public void UpdateProductDetails(string name, string description, ValueObjects.Quantity quantity, ValueObjects.Price price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Product description cannot be empty.");
            if (quantity == null)
                throw new DomainException("Product quantity cannot be null.");
            if (price == null)
                throw new DomainException("Product price cannot be null.");

            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;

            // Add domain event: ProductDetailsUpdatedEvent
        }

        /// <summary>
        /// Reduces the quantity of the product (e.g., after a sale or transport).
        /// </summary>
        /// <param name="amount">The amount to reduce.</param>
        public void ReduceQuantity(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount to reduce must be positive.");
            if (Quantity.Value < amount)
                throw new DomainException("Insufficient product quantity.");

            Quantity = new ValueObjects.Quantity(Quantity.Value - amount, Quantity.Unit);
            // Add domain event: ProductQuantityReducedEvent
        }

        /// <summary>
        /// Increases the quantity of the product (e.g., new stock).
        /// </summary>
        /// <param name="amount">The amount to increase.</param>
        public void IncreaseQuantity(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount to increase must be positive.");

            Quantity = new ValueObjects.Quantity(Quantity.Value + amount, Quantity.Unit);
            // Add domain event: ProductQuantityIncreasedEvent
        }
    }
}