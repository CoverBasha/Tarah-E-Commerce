using System.ComponentModel.DataAnnotations;

namespace Tarah.API.Models.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterSale { get; set; }
        public bool IsOnSale { get; set; }
        public List<CategoryItem> Categories { get; set; }
        public List<string>? Images { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
