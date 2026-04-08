using System.ComponentModel.DataAnnotations;

namespace Tarah.API.Models.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemDto
    {
        public ProductDto Product { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
