using Greenlogist.Backend.Greenlogist.Domain.Aggregates.Shipping;
using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;
using Greenlogist.Domain.ValueObjects; // For Quantity, Location

namespace Greenlogist.Domain.Aggregates.Shipping
{
    public enum ShippingStatus
    {
        Pending,        // Request submitted, awaiting confirmation/assignment
        Scheduled,      // Transport scheduled
        InProgress,     // Transport is on its way
        Completed,      // Transport finished, product delivered
        Cancelled       // Transport cancelled
    }

    /// <summary>
    /// Represents the 'ShippingRequest' aggregate root entity.
    /// </summary>
    public class ShippingRequest : Entity, IAggregateRoot
    {
        public Guid ProducerId { get; private set; }
        public Guid ProductId { get; private set; } // The product being transported
        public Quantity Quantity { get; private set; } // Quantity of the product to transport
        public ValueObjects.Location Origin { get; private set; } // Origin location (as a value object)
        public ValueObjects.Location Destination { get; private set; } // Destination location (as a value object)
        public DateTime RequiredDate { get; private set; } // Date requested for transport
        public string? SpecialInstructions { get; private set; }
        public ShippingStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Private constructor for reconstruction from persistence (DDD)
        private ShippingRequest() { }

        /// <summary>
        /// Constructor for creating a new shipping request.
        /// </summary>
        /// <param name="id">Unique identifier for the shipping request.</param>
        /// <param name="producerId">ID of the producer making the request.</param>
        /// <param name="productId">ID of the product to be transported.</param>
        /// <param name="quantity">Quantity of the product (as a value object).</param>
        /// <param name="origin">Origin location (as a value object).</param>
        /// <param name="destination">Destination location (as a value object).</param>
        /// <param name="requiredDate">Requested date for transport.</param>
        /// <param name="specialInstructions">Special instructions for the transport.</param>
        public ShippingRequest(Guid id, Guid producerId, Guid productId, Quantity quantity, ValueObjects.Location origin, ValueObjects.Location destination, DateTime requiredDate, string? specialInstructions) : base(id)
        {
            if (producerId == Guid.Empty)
                throw new DomainException("Producer ID cannot be empty.");
            if (productId == Guid.Empty)
                throw new DomainException("Product ID cannot be empty.");
            if (quantity == null)
                throw new DomainException("Quantity cannot be null.");
            if (origin == null)
                throw new DomainException("Origin location cannot be null.");
            if (destination == null)
                throw new DomainException("Destination location cannot be null.");
            if (requiredDate == default(DateTime) || requiredDate < DateTime.UtcNow.Date) // Required date cannot be in the past
                throw new DomainException("Required date must be a valid future date.");

            ProducerId = producerId;
            ProductId = productId;
            Quantity = quantity;
            Origin = origin;
            Destination = destination;
            RequiredDate = requiredDate;
            SpecialInstructions = specialInstructions;
            Status = ShippingStatus.Pending; // Initial status
            CreatedAt = DateTime.UtcNow;

            // Add domain event: TransportRequestedEvent
            // AddDomainEvent(new TransportRequestedEvent(Id, ProducerId, ProductId, Quantity.Value, Quantity.Unit, Origin.Address, Destination.Address, RequiredDate));
        }

        /// <summary>
        /// Updates the status of the shipping request.
        /// </summary>
        /// <param name="newStatus">The new status for the shipping request.</param>
        public void UpdateStatus(ShippingStatus newStatus)
        {
            // Add business rules for status transitions (e.g., cannot go from Completed to Pending)
            if (Status == ShippingStatus.Completed || Status == ShippingStatus.Cancelled)
            {
                throw new DomainException("Cannot change status of a completed or cancelled shipping request.");
            }
            if (newStatus < Status) // Simple example: cannot go backwards in status progression
            {
                throw new DomainException($"Cannot change status from {Status} to {newStatus}.");
            }

            Status = newStatus;
            // Add domain event: ShippingStatusUpdatedEvent
        }

        // You could add methods for rescheduling, cancelling, etc.
    }
}
