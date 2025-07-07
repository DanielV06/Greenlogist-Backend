namespace Greenlogist.Application.DTOs.Order
{
    /// <summary>
    /// DTO for an item within an order (read model).
    /// </summary>
    public record OrderItemDto(
        Guid ProductId,
        string ProductName,
        decimal Quantity,
        string Unit,
        decimal UnitPrice,
        string Currency
    );
}
