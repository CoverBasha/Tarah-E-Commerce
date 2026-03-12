using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class CheckoutTransaction
    {
        private readonly TarahDbContext context;

        public CheckoutTransaction(TarahDbContext context)
        {
            this.context = context;
        }

        public async Task<Order> Execute(Guid userId)
        {
            var user = await context.Customers.Include(o=>o.PastOrders).SingleOrDefaultAsync(c => c.Id == userId);
            if (user is null)
                throw new Exception("user doesn't exist");

            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product).SingleOrDefaultAsync(c => c.CustomerId == userId);

            if (cart.Items.IsNullOrEmpty())
                throw new Exception("Cart is empty");

            foreach (var item in cart.Items)
            {
                var product = await context.Products.SingleOrDefaultAsync(p => p.Id == item.ProductId);
                if (product == null)
                    throw new Exception("Product not found");

                if (product.Stock < item.Quantity)
                    throw new Exception("Quantity exceeds stock");

                product.Stock -= item.Quantity;
            }

            var order = new Order
            {
                CreatedAt = DateTime.Now,
            };

            order.Items = cart.Items.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                UnitPrice = c.Product.IsOnSale ? c.Product.PriceAfterSale : c.Product.Price,
                Quantity = c.Quantity,
                ProductName = c.Product.Name,
                Order = order,
            }).ToList();
            order.TotalPrice = order.Items.Sum(p => p.UnitPrice * p.Quantity);
            order.CreatedAt = DateTime.Now;

            user.PastOrders.Add(order);

            cart.Items = new();

            await context.SaveChangesAsync();

            return order;
        }
    }
}
