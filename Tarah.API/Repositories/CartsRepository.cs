using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        private readonly TarahDbContext context;

        public CartsRepository(TarahDbContext context)
        {
            this.context = context;
        }

        public async Task<Cart> GetCart(Guid userId)
        {
            var cart = await context.Carts
                .Include(c=>c.Items)
                .SingleOrDefaultAsync(c => c.CustomerId == userId);
            return cart;
        }

        public async Task<Cart> GetFullCart(Guid userId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(p => p.Product)
                .ThenInclude(c => c.Categories)
                .ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(c => c.CustomerId == userId);
            return cart;
        }

        public async Task<Cart> AddToCart(Cart cart, Product product, int quantity)
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                Quantity = quantity,
                ProductId = product.Id
            });
            await context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> ModifyItemCount(Cart cart, Product product, int quantity)
        {
            var cartItem = await context.CartItems
                .SingleOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == product.Id);
            if (cartItem is null)
                return false;

            cartItem.Quantity = quantity;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItem(Cart cart, Product product)
        {
            var item = await context.CartItems
                .SingleOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == product.Id);

            if (item is null)
                return false;

            context.CartItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(Cart cart)
        {
            cart.Items = new();
            await context.SaveChangesAsync();
            return true;
        }
    }
}
