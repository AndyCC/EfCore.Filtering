﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleApi.Data.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ShopProductListing> ProductListings { get; set; } = null!;
        public List<ShippingRegion> ShippingRegions { get; set; } = null!;
    }

    public static class ShopModelBuilder
    {
        public static void Build(EntityTypeBuilder<Shop> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(x => x.ProductListings)
                .WithOne(x => x.Shop)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ShippingRegions)
                .WithMany(x => x.Shops);
        }
    }
}
