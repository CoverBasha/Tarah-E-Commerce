using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface ICartsRepository
    {
        public Task<Cart> GetCart(Guid userId);
        public Task<Cart> GetFullCart(Guid userId);
        public Task<Cart> AddToCart(Cart cart, Product product, int quantity);
        public Task<bool> ModifyItemCount(Cart cart, Product product, int quantity);
        public Task<bool> RemoveItem(Cart cart, Product product);
        public Task<bool> ClearCart(Cart cart);
    }
}
