namespace Tarah.API.Models.Domain
{
    public class CategoryItem
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}