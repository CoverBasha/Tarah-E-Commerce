using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface ICategoriesRepository
    {
        public Task<List<Category>> AllCategories();
    }
}
