using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly TarahDbContext context;

        public CategoriesRepository(TarahDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Category>> AllCategories()
        {
            return await context.Categories.ToListAsync();
        }
    }
}
