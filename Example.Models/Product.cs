using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json.Serialization;

namespace ExampleApi.Data.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ShopProductListing> Listings { get; set; } = null!;
    }

    public static class ProductModelBuilder
    {
        public static void Build(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(x => x.Listings)
                .WithOne(x => x.Product)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
