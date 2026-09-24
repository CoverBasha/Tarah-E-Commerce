using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface ICategoriesRepository
    {
        public Task<List<Category>> AllCategories();
        public Task<List<Category>> ListCategoriesIds(IEnumerable<Guid> ids);
        public Task<Category> CategoryById(Guid id);
        public Task<PagedResult<Product>> GetProductsByCategoryAsync(Guid categoryId, int page, int pageSize);

    }
}
