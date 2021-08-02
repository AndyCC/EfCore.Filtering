using EfCore.Filtering.Client;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.EfCore.Filtering.TestDb;
using Tests.EfCore.Filtering.TestDb.Models;
using static Tests.EfCore.Filtering.TestDb.TestData;

namespace Tests.EfCore.Filtering
{
    public class OrderByTests : QueryBuilderTestBase
    {
        [Test]
        public async Task OrdersByAscending()
        {
            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Product>
                    {
                        PathExpression = x => x.Id,
                        Order = Ordering.ASC
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count(), Is.EqualTo(TestData.Products.Count()));

            var expectedId = TestData.Products.Min(x => x.Id);
            foreach (var product in results)
            {
                Assert.That(product.Id, Is.EqualTo(expectedId));
                expectedId++;
            }
        }

        [Test]
        public async Task OrdersByDescending()
        {
            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Product>
                    {
                        PathExpression = x => x.Id,
                        Order = Ordering.DESC
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count(), Is.EqualTo(TestData.Products.Count()));

            var expectedId = TestData.Products.Max(x => x.Id);
            foreach (var product in results)
            {
                Assert.That(product.Id, Is.EqualTo(expectedId));
                expectedId--;
            }
        }

        [Test]
        public async Task OrdersByAPropertyAssociationAndThenByAnotherPropertyAndAlsoIncludesWithOrderBy()
        {
            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<ShopProductListing>
                    {
                        PathExpression = x => x.Product.Name,
                        Order = Ordering.ASC
                    },
                    new OrderBy<ShopProductListing>
                    {
                        PathExpression = x => x.AnotherValue,
                        Order = Ordering.DESC,
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);

            var results = await query(DbContext.ShopProductListings).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));

            var expectedData = TestData.Shops
                .SelectMany(shop => shop.ProductListings)
                .OrderBy(listing => listing.Product.Name)
                .ThenByDescending(listing => listing.AnotherValue)
                .ToArray();

            Assert.That(results.Count(), Is.EqualTo(expectedData.Length));

            for(var i=0; i<results.Count(); i++)
                Assert.That(results[i].AnotherValue, Is.EqualTo(expectedData[i].AnotherValue));   
        }

        [Test]
        public async Task AppliesSkipAndTakeWithOrderBy()
        {
            var filter = new Filter
            {
                Skip = 1,
                Take = 1,
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Product>
                    {
                        PathExpression = x => x.Id,
                        Order = Ordering.DESC
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count(), Is.EqualTo(1));
            var expectedId = TestData.Products.Max(x => x.Id) - 1;
            Assert.That(results[0].Id, Is.EqualTo(expectedId));
        }

        [Test]
        public async Task OrderByPropertyOnNavigatingThroughACollection()
        {
            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Shop>
                    {
                        Path = "ProductListings.Product.Id",
                        Order = Ordering.DESC,
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].Name, Is.EqualTo(ShopNames.Everythings10Shop));
            Assert.That(results[1].Name, Is.EqualTo(ShopNames.Enviromental));
            Assert.That(results[2].Name, Is.EqualTo(ShopNames.AnimalShop));
        }

        [Test]
        public async Task OrderByPropertyOnNavigatingThroughMultpleCollections()
        {
            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Product>
                    {
                        Path = "Listings.Shop.ShippingRegions.Id",
                        Order = Ordering.DESC,
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);
            var results = await query(DbContext.Products).ToListAsync();

            Assert.That(results.Count, Is.EqualTo(7));
            Assert.That(results[0].Name, Is.EqualTo(ProductNames.CatDispenser));
            Assert.That(results[1].Name, Is.EqualTo(ProductNames.DogDispatcher));
            Assert.That(results[2].Name, Is.EqualTo(ProductNames.UmbrellaStand));
            Assert.That(results[3].Name, Is.EqualTo(ProductNames.CarpetCleaner));
            Assert.That(results[4].Name, Is.EqualTo(ProductNames.SnowMachine));
            Assert.That(results[5].Name, Is.EqualTo(ProductNames.SummerInstaller));
            Assert.That(results[6].Name, Is.EqualTo(ProductNames.DinnerMakerRobot));
        }

    }
}
