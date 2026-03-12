using Tarah.API.Models.Domain;

namespace Tarah.API.Models.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterSale { get; set; }
        public bool IsOnSale { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<string> Images { get; set; }
    }

    public class ListProductsDto
    {
        public List<ProductDto> Dtos { get; set; }
        public decimal Count { get; set; }
    }

    public class AddProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterSale { get; set; }
        public bool IsOnSale { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
