using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface IOrdersRepository
    {
        public Task<List<Order>> AllOrders(Guid userId, int page, int pageSize);
        public Task<Order> OrderById(Guid userId, Guid orderId);
    }
}
