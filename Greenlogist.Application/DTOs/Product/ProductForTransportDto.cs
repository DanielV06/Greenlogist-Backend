namespace Greenlogist.Application.DTOs.Product
{
    /// <summary>
    /// DTO for products available for transport (read model).
    /// </summary>
    public record ProductForTransportDto(
        Guid Id,
        string Name,
        decimal Quantity,
        string Unit
    );
}
