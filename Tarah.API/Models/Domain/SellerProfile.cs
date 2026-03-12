namespace Tarah.API.Models.Domain
{
    public class SellerProfile
    {
        public Guid Id { get; set; }
        public List<Product>? Products { get; set; }
    }
}
