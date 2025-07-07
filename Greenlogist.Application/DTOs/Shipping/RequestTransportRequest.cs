using System.ComponentModel.DataAnnotations;

namespace Greenlogist.Application.DTOs.Shipping
{
    /// <summary>
    /// DTO for the request to solicit transport.
    /// </summary>
    public class RequestTransportRequest
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity value is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal QuantityValue { get; set; }

        [Required(ErrorMessage = "Quantity unit is required.")]
        public string QuantityUnit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Origin address is required.")]
        public string OriginAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Origin city is required.")]
        public string OriginCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Origin country is required.")]
        public string OriginCountry { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination address is required.")]
        public string DestinationAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination city is required.")]
        public string DestinationCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination country is required.")]
        public string DestinationCountry { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required date is required.")]
        public DateTime RequiredDate { get; set; }

        public string? SpecialInstructions { get; set; }
    }
}
