using System.ComponentModel.DataAnnotations;

namespace Greenlogist.Application.DTOs.Product
{
    /// <summary>
    /// DTO for the product registration request.
    /// </summary>
    public class RegisterProductRequest
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity value is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal QuantityValue { get; set; }

        [Required(ErrorMessage = "Quantity unit is required.")]
        public string QuantityUnit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price value is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal PriceValue { get; set; }

        [Required(ErrorMessage = "Price currency is required.")]
        public string PriceCurrency { get; set; } = string.Empty;
    }
}
