using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly TarahDbContext context;

        public OrdersRepository(TarahDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Order>> AllOrders(Guid userId, int page, int pageSize)
        {
            IQueryable<Order> orders = context.Orders.Where(o => o.UserId == userId);

            orders = orders.Skip((page - 1) * pageSize).Take(pageSize);

            return await orders.Include(i => i.Items).ToListAsync();
        }
        public async Task<Order> OrderById(Guid userId, Guid orderId)
        {
            return await context.Orders.Include(i => i.Items).SingleOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }
    }
}
