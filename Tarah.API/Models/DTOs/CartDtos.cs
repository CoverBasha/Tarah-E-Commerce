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
        public int Quantity { get; set; }
    }
}
