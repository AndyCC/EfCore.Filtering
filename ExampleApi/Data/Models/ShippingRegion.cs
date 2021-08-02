using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace ExampleApi.Data.Models
{
    public class ShippingRegion
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public List<Shop> Shops { get; set; }
    }

    public static class ShippingRegionBuilder
    {
        public static void Build(EntityTypeBuilder<ShippingRegion> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Region)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(x => x.Shops)
                .WithMany(x => x.ShippingRegions);
        }
    }
}
