using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface IProductsRepository
    {
        public Task<PagedResult<Product>> GetProductsAsync(Guid categoryId, int page, int pageSize);
        public Task<Product>? GetByIdAsync(Guid Id);
        public Task<Product> AddProductAsync(Product product);
        public Task<string> SaveImage(IFormFile file);
        public Task<Product> UpdateProductAsync(Guid id, Product product);
        public Task<bool> DeleteAsync(Product product);
        public void DeleteImage(string fileName);
        public Task Commit();
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }
    }
}
