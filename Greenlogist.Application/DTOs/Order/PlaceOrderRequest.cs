using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Greenlogist.Application.DTOs.Order
{
    /// <summary>
    /// DTO for the request to place a new order.
    /// </summary>
    public class PlaceOrderRequest
    {
        [Required(ErrorMessage = "Consumer ID is required.")]
        public Guid ConsumerId { get; set; }

        [Required(ErrorMessage = "Producer ID is required.")]
        public Guid ProducerId { get; set; }

        [Required(ErrorMessage = "Order must contain at least one item.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}