using System.ComponentModel.DataAnnotations;

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
        [Length(1,255)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value must be positive")]
        public int Stock { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be positive")]
        public decimal Price { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be positive")]
        public decimal PriceAfterSale { get; set; }
        public bool IsOnSale { get; set; }
        [MinLength(1, ErrorMessage = "Product should have atleast one category")]
        public List<Guid> CategoryIds { get; set; }
        [MinLength(1,ErrorMessage = "Product should have atleast one Image")]
        public List<IFormFile> Images { get; set; }
    }

    public class UpdateProductDto
    {
        [Length(1, 255)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value must be positive")]
        public int Stock { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be positive")]
        public decimal Price { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be positive")]
        public decimal PriceAfterSale { get; set; }
        public bool IsOnSale { get; set; }
        [MinLength(1, ErrorMessage = "Product should have atleast one category")]
        public List<Guid> CategoryIds { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
