using Greenlogist.Domain.Common;
using Greenlogist.Domain.Exceptions;
using Greenlogist.Domain.ValueObjects; // For Price and Quantity (reused from Product context)
using System.Collections.Generic;
using System.Linq;

namespace Greenlogist.Domain.Aggregates.Order
{
    public enum OrderStatus
    {
        Pending,        // Order placed, awaiting payment/confirmation
        Paid,           // Payment received
        Processing,     // Order being prepared/fulfilled
        Shipped,        // Order has been shipped (could link to ShippingRequest)
        Delivered,      // Order successfully delivered to consumer
        Cancelled,       // Order cancelled
        Completed
    }

    /// <summary>
    /// Represents the 'Order' aggregate root entity.
    /// </summary>
    public class Order : Entity, IAggregateRoot
    {
        public Guid ConsumerId { get; private set; }
        public Guid ProducerId { get; private set; } // The producer who will fulfill this order
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; } // Calculated total amount of the order

        // Collection of OrderItems as part of the aggregate
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        // Private constructor for reconstruction from persistence (DDD)
        private Order()
        {
            _orderItems = new List<OrderItem>();
        }

        /// <summary>
        /// Constructor for creating a new order.
        /// </summary>
        /// <param name="id">Unique identifier for the order.</param>
        /// <param name="consumerId">ID of the consumer who placed the order.</param>
        /// <param name="producerId">ID of the producer fulfilling the order.</param>
        /// <param name="orderItems">List of items included in the order.</param>
        public Order(Guid id, Guid consumerId, Guid producerId, IEnumerable<OrderItem> orderItems) : base(id)
        {
            if (consumerId == Guid.Empty)
                throw new DomainException("Consumer ID cannot be empty.");
            if (producerId == Guid.Empty)
                throw new DomainException("Producer ID cannot be empty.");
            if (orderItems == null || !orderItems.Any())
                throw new DomainException("Order must contain at least one item.");

            ConsumerId = consumerId;
            ProducerId = producerId;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending; // Initial status
            _orderItems = new List<OrderItem>(orderItems);
            CalculateTotalAmount();

            // Add domain event: OrderPlacedEvent
            // AddDomainEvent(new OrderPlacedEvent(Id, ConsumerId, ProducerId, TotalAmount, OrderDate));
        }

        /// <summary>
        /// Adds an item to the order.
        /// </summary>
        /// <param name="orderItem">The order item to add.</param>
        public void AddItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new DomainException("Order item cannot be null.");

            _orderItems.Add(orderItem);
            CalculateTotalAmount(); // Recalculate total when items change
        }

        /// <summary>
        /// Updates the status of the order.
        /// </summary>
        /// <param name="newStatus">The new status for the order.</param>
        public void UpdateStatus(OrderStatus newStatus)
        {
            // Add business rules for status transitions (e.g., cannot go from Cancelled to Paid)
            if (Status == OrderStatus.Cancelled || Status == OrderStatus.Delivered)
            {
                throw new DomainException("Cannot change status of a cancelled or delivered order.");
            }
            // Example: Order must be Pending to become Paid
            if (newStatus == OrderStatus.Paid && Status != OrderStatus.Pending)
            {
                throw new DomainException("Order must be in 'Pending' status to be marked as 'Paid'.");
            }
            // You can add more complex rules here

            Status = newStatus;
            // Add domain event: OrderStatusUpdatedEvent
        }

        /// <summary>
        /// Calculates the total amount of the order based on its items.
        /// </summary>
        private void CalculateTotalAmount()
        {
            TotalAmount = _orderItems.Sum(item => item.Quantity.Value * item.UnitPrice.Value);
        }
    }
}
