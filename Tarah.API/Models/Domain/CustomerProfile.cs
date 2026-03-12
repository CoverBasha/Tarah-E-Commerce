namespace Tarah.API.Models.Domain
{
    public class CustomerProfile
    {
        public Guid Id { get; set; }
        
        public Cart? Cart { get; set; }
        public List<Order>? PastOrders { get; set; }
    }
}
