using MediatR;

namespace Greenlogist.Application.Commands.Shipping
{
    /// <summary>
    /// Command to solicit a new transport request.
    /// </summary>
    public record SolicitTransportCommand(
        Guid ProducerId,
        Guid ProductId,
        decimal QuantityValue,
        string QuantityUnit,
        string OriginAddress,
        string OriginCity,
        string OriginCountry,
        string DestinationAddress,
        string DestinationCity,
        string DestinationCountry,
        DateTime RequiredDate,
        string? SpecialInstructions
    ) : IRequest<Guid>; // Returns the ID of the newly created shipping request
}
