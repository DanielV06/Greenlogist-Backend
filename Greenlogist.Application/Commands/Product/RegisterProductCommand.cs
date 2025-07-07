using MediatR;

namespace Greenlogist.Application.Commands.Product
{
    /// <summary>
    /// Command to register a new product.
    /// </summary>
    public record RegisterProductCommand(
        Guid ProducerId,
        string Name,
        string Description,
        decimal QuantityValue,
        string QuantityUnit,
        decimal PriceValue,
        string PriceCurrency
    ) : IRequest<Guid>; // Returns the ID of the newly registered product
}
