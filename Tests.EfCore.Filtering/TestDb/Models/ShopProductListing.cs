﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tests.EfCore.Filtering.TestDb.Models
{
    public class ShopProductListing
    {
        public Shop Shop { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public int AnotherValue { get; set; }
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
                .HasColumnType("Money")
                .IsRequired();
        }
    }
}
