using MediatR;
using Greenlogist.Application.DTOs.Order; // For OrderItemRequest

namespace Greenlogist.Application.Commands.Order
{
    /// <summary>
    /// Command to place a new order.
    /// </summary>
    public record PlaceOrderCommand(
        Guid ConsumerId,
        Guid ProducerId,
        IEnumerable<OrderItemRequest> Items
    ) : IRequest<Guid>; // Returns the ID of the newly placed order
}
