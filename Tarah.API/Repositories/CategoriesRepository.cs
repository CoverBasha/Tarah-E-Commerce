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

        public Task<Category> CategoryById(Guid id)
        {
            return context.Categories.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Category>> ListCategoriesIds(IEnumerable<Guid> ids)
        {
            return await context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
        }

        public Task<PagedResult<Product>> GetProductsByCategoryAsync(Guid categoryId, int page, int pageSize)
        {
            IQueryable<Product> query = context.Products;
            query = query.Where(p => p.Categories.Any(c => c.CategoryId == categoryId));
            decimal pages = query.Count();
            pages /= pageSize;
            query = query.Skip((page - 1) * pageSize).Take(pageSize);


            return Task.FromResult(new PagedResult<Product>
            {
                Items = query.Include(c => c.Categories)
                .ThenInclude(c => c.Category).ToList(),
                TotalPages = (int)Math.Ceiling(pages)
            });
        }

    }
}
