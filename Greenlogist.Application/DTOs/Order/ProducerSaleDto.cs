using System.Collections.Generic;

namespace Greenlogist.Application.DTOs.Order
{
    /// <summary>
    /// DTO for a producer's sale (read model for producer's sales history).
    /// </summary>
    public record ProducerSaleDto(
        Guid Id,
        Guid ConsumerId, // To identify who bought it
        DateTime OrderDate,
        string Status,
        decimal TotalAmount,
        IReadOnlyCollection<OrderItemDto> Items // Details of products sold in this order
    );
}
