using ExampleApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Data.Seeding
{
    public static class SeedData
    {
        public static async Task SeedDataAndClearChangesAsync(this ShoppingDbContext context)
        {
            await context.AddRangeAsync(Products);
            await context.AddRangeAsync(ShippingRegions);
            await context.SaveChangesAsync();

            var shops = await BuildShops(context);
            await context.AddRangeAsync(shops);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        public static IEnumerable<Product> Products
        {
            get
            {
                return new Product[]
                {
                    new Product { Id = 1, Name = ProductNames.CatDispenser },
                    new Product { Id = 2, Name = ProductNames.DogDispatcher },
                    new Product { Id = 3, Name = ProductNames.UmbrellaStand },
                    new Product { Id = 4, Name = ProductNames.CarpetCleaner },
                    new Product { Id = 5, Name = ProductNames.SnowMachine },
                    new Product { Id = 6, Name = ProductNames.SummerInstaller },
                    new Product { Id = 7, Name = ProductNames.DinnerMakerRobot },
                };
            }
        }

        public static IEnumerable<ShippingRegion> ShippingRegions
        {
            get
            {
                return new ShippingRegion[]
                {
                    new ShippingRegion { Id = 1,  Region = RegionNames.UK },
                    new ShippingRegion { Id = 2,  Region = RegionNames.EU }
                };
            }
        }

        private static async Task<IEnumerable<Shop>> BuildShops(ShoppingDbContext context)
        {
            Shops = new Shop[]
            {
                    new Shop
                    {
                       Id = 1,
                       Name = ShopNames.Everythings10Shop,
                       ProductListings = new List<ShopProductListing>
                       {
                           new ShopProductListing { Id = 1, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.CarpetCleaner) },
                           new ShopProductListing { Id = 2, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.CatDispenser) },
                           new ShopProductListing { Id = 3, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.DinnerMakerRobot) },
                           new ShopProductListing { Id = 4, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.DogDispatcher) },
                           new ShopProductListing { Id = 5, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.SnowMachine) },
                           new ShopProductListing { Id = 6, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.SummerInstaller) },
                           new ShopProductListing { Id = 7, Price = 10, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.UmbrellaStand) },
                       },
                       ShippingRegions = new List<ShippingRegion>
                       {
                           context.ShippingRegions.Single(x => x.Region == RegionNames.UK)
                       }
                    },
                    new Shop
                    {
                        Id = 2,
                        Name = ShopNames.AnimalShop,
                        ProductListings = new List<ShopProductListing>
                        {
                           new ShopProductListing {Id = 8, Price = 13, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.CatDispenser) },
                           new ShopProductListing {Id = 9, Price = 9, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.DogDispatcher) },
                        },
                       ShippingRegions = new List<ShippingRegion>
                       {
                           await context.ShippingRegions.SingleAsync(x => x.Region == RegionNames.UK),
                           await context.ShippingRegions.SingleAsync(x => x.Region == RegionNames.EU),
                       }
                    },
                    new Shop
                    {
                        Id = 3,
                        Name = ShopNames.Enviromental,
                        ProductListings = new List<ShopProductListing>
                        {
                           new ShopProductListing { Id = 10, Price = 15, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.SnowMachine) },
                           new ShopProductListing { Id = 11, Price = 25, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.SummerInstaller) },
                           new ShopProductListing { Id = 12, Price = 8, Product = await context.Products.SingleAsync(x => x.Name == ProductNames.CatDispenser) },
                        },
                        ShippingRegions = new List<ShippingRegion>
                        {
                           await context.ShippingRegions.SingleAsync(x => x.Region == RegionNames.UK),
                           await context.ShippingRegions.SingleAsync(x => x.Region == RegionNames.EU),
                        }
                    }
            };

            foreach (var shop in Shops)
            {
                foreach (var listing in shop.ProductListings)
                    listing.Shop = shop;
            }

            return Shops;

        }

        public static IEnumerable<Shop> Shops { get; private set; }

        public static class ProductNames
        {
            public const string CatDispenser = "Cat Dispenser";
            public const string DogDispatcher = "Dog Dispatcher";
            public const string UmbrellaStand = "Umbrella Stand";
            public const string CarpetCleaner = "Carpet Cleaner";
            public const string SnowMachine = "Snow Machine";
            public const string SummerInstaller = "Summer Installer";
            public const string DinnerMakerRobot = "Dinner Maker Robot";
        }

        public static class RegionNames
        {
            public const string UK = "UK";
            public const string EU = "EU";
        }

        public static class ShopNames
        {
            public const string Everythings10Shop = "Everythings 10 Shop";
            public const string AnimalShop = "Animal Shop";
            public const string Enviromental = "Enviromental";
        }

    }
}
