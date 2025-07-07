using System.ComponentModel.DataAnnotations;

namespace Greenlogist.Application.DTOs.Order
{
    /// <summary>
    /// DTO for an item within an order request.
    /// </summary>
    public class OrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required for an order item.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity value is required for an order item.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0 for an order item.")]
        public decimal QuantityValue { get; set; }

        [Required(ErrorMessage = "Quantity unit is required for an order item.")]
        public string QuantityUnit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit price value is required for an order item.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0 for an order item.")]
        public decimal UnitPriceValue { get; set; }

        [Required(ErrorMessage = "Unit price currency is required for an order item.")]
        public string UnitPriceCurrency { get; set; } = string.Empty;
    }
}