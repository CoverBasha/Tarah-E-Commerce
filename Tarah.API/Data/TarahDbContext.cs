using Microsoft.EntityFrameworkCore;
using Tarah.API.Models.Domain;

namespace Tarah.API.Data
{
    public class TarahDbContext : DbContext
    {
        public DbSet<CustomerProfile> Customers { get; set; }
        public DbSet<SellerProfile> Sellers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryItem> CategoryItems { get; set; }
        public DbSet<DeletedUser> DeletedUsers { get; set; }
        public DbSet<LocalUser> LocalUsers { get; set; }


        public TarahDbContext(DbContextOptions<TarahDbContext> options) : base(options) { }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => new { ci.CartId, ci.ProductId });

                entity.HasOne(ci => ci.Cart)
                      .WithMany(c => c.Items)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Order>()
                        .HasMany(o => o.Items)
                        .WithOne(i => i.Order)
                        .HasForeignKey(i => i.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryItem>(entity =>
            {
                entity.HasKey(k => new { k.ProductId, k.CategoryId });

                entity.HasOne(p => p.Product)
                      .WithMany(ci => ci.Categories)
                      .HasForeignKey(p => p.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Category)
                      .WithMany(ci => ci.Products)
                      .HasForeignKey(c => c.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
