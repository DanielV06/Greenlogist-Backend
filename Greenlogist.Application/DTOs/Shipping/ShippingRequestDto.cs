namespace Greenlogist.Application.DTOs.Shipping
{
    /// <summary>
    /// DTO for a shipping request (read model).
    /// </summary>
    public record ShippingRequestDto(
        Guid Id,
        Guid ProductId,
        string ProductName, // Denormalized for read model
        decimal Quantity,
        string Unit,
        string OriginAddress,
        string DestinationAddress,
        DateTime RequiredDate,
        string? SpecialInstructions,
        string Status,
        DateTime CreatedAt
    );
}
