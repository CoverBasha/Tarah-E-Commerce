namespace Tarah.API.Models.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CategoryItem> Products { get; set; }
    }
}
