using ExampleApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleApi.Data
{
    public class ShoppingDbContext : DbContext
    {
        public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
          : base(options)
        {

        }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShopProductListing> ShopProductListings { get; set; }
        public DbSet<ShippingRegion> ShippingRegions { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(ProductModelBuilder.Build);
            modelBuilder.Entity<ShopProductListing>(ShopProductListingModelBuilder.Build);
            modelBuilder.Entity<Shop>(ShopModelBuilder.Build);
            modelBuilder.Entity<ShippingRegion>(ShippingRegionBuilder.Build);
        }
    }
}
