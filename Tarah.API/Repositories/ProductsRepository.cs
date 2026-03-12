using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly TarahDbContext context;
        private readonly string imagesDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

        public ProductsRepository(TarahDbContext context)
        {
            this.context = context;
        }

        public async Task<PagedResult<Product>> GetProductsAsync(Guid categoryId, int page, int pageSize)
        {
            IQueryable<Product> query = context.Products;

            if (categoryId != Guid.Empty)
                query = query.Where(c => c.Categories.Any(i => i.CategoryId == categoryId));

            decimal pages = await query.CountAsync();
            pages /= pageSize;

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new PagedResult<Product>
            {
                Items = await query.ToListAsync(),
                TotalPages = (int)Math.Ceiling(pages)
            };
        }

        public async Task<Product>? GetByIdAsync(Guid Id)
        {
            return await context.Products
                .Include(c => c.Categories).
                ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateProductAsync(Guid id, Product product)
        {
            var productInDb = await context.Products.SingleOrDefaultAsync(p => p.Id == id);

            productInDb = product;

            await context.SaveChangesAsync();

            return productInDb;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            foreach (var image in product.Images)
                DeleteImage(image);

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            if (!Path.Exists(imagesDirectory))
                Directory.CreateDirectory(imagesDirectory);

            var name = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);
            name += extension;

            var path = Path.Combine(imagesDirectory, name);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return name;
        }

        public void DeleteImage(string fileName)
        {
            var file = Path.Combine(imagesDirectory, fileName);
            if (File.Exists(file))
                File.Delete(file);
        }

        public async Task Commit()
        {
            await context.SaveChangesAsync();
        }
    }
}
