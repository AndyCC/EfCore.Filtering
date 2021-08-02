using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExampleApi.Data.Models
{
    public class ShopProductListing
    {
        public int Id { get; set; }
        public Shop Shop { get; set; }
        public Product Product { get; set; }
        public int Price { get; set; }
    }

    public static class ShopProductListingModelBuilder
    {
        public static void Build(EntityTypeBuilder<ShopProductListing> builder)
        {
            builder.HasKey("ShopId", "ProductId");

            builder.HasOne(x => x.Shop)
                .WithMany(x => x.ProductListings)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Listings)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Price)
                .IsRequired();
        }
    }
}
