using System.Collections.Generic;

namespace Greenlogist.Application.DTOs.Order
{
    /// <summary>
    /// DTO for an order (read model for consumer's order history).
    /// </summary>
    public record OrderDto(
        Guid Id,
        Guid ConsumerId,
        Guid ProducerId,
        DateTime OrderDate,
        string Status,
        decimal TotalAmount,
        IReadOnlyCollection<OrderItemDto> Items
    );
}
