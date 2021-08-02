using EfCore.Filtering.Client;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.EfCore.Filtering.TestDb;
using Tests.EfCore.Filtering.TestDb.Models;
using static Tests.EfCore.Filtering.TestDb.TestData;

namespace Tests.EfCore.Filtering
{
    public class IncludeTests : QueryBuilderTestBase
    {
        [Test]
        public async Task IncludesSpecifiedModelWhenIncludingSingleEntityProperty()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShopProductListing>
                    {
                        PathExpression = x => x.Shop,
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);
            var results = await query(DbContext.ShopProductListings).ToListAsync();

            foreach (var result in results)
            {
                Assert.IsNull(result.Product);
                Assert.IsNotNull(result.Shop);
            }

        }

        [Test]
        public async Task IncludesSpecifiedModelWhenIncludingListProperty()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            foreach (var result in results)
            {
                Assert.IsNotNull(result.ProductListings);
                Assert.IsNull(result.ShippingRegions);
                Assert.GreaterOrEqual(result.ProductListings.Count, 1);
            }
        }

        [Test]
        public async Task ThenIncludeAddedForAListPropertyAfterASingleProperty()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShopProductListing>
                    {
                        PathExpression = x => x.Shop,
                        Filter = new Filter
                        {
                            Includes = new List<Include>
                            {
                                new Include<Shop>
                                {
                                    PathExpression = x => x.ShippingRegions
                                }
                            }
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);
            var results = await query(DbContext.ShopProductListings).ToListAsync();

            foreach (var productListing in results)
            {
                Assert.IsNotNull(productListing.Shop);
                Assert.IsNotNull(productListing.Shop.ShippingRegions);

                foreach (var region in productListing.Shop.ShippingRegions)
                    Assert.IsNotNull(region);
            }
        }

        [Test]
        public async Task ThenIncludeAddedForASinglePropertyAfterAListProperty()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Includes = new List<Include>
                            {
                                new Include<ShopProductListing>
                                {
                                    PathExpression = x => x.Product
                                }
                            }
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            foreach (var shop in results)
            {
                Assert.IsNotNull(shop.ProductListings);

                foreach (var listing in shop.ProductListings)
                    Assert.IsNotNull(listing.Product);                
            }
        }

        [Test]
        public async Task IncludeWillFilterOnSkip()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Skip = 1,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            foreach (var result in results)           
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == result.Name);

                Assert.IsNotNull(result.ProductListings);
                Assert.IsNull(result.ShippingRegions);
                Assert.GreaterOrEqual(result.ProductListings.Count, expectedShop.ProductListings.Count - 1);
            }
        }

        [Test]
        public async Task IncludeWillFilterOnTake()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Take = 1,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();
            Assert.That(results.Count, Is.EqualTo(3));

            foreach (var result in results)
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == result.Name);

                Assert.IsNotNull(result.ProductListings);
                Assert.IsNull(result.ShippingRegions);
                Assert.That(result.ProductListings.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task IncludeWillFilterOnSkipAndTake()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Skip = 1,
                            Take = 1,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();
            Assert.That(results.Count, Is.EqualTo(3));

            foreach (var result in results)
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == result.Name);

                Assert.IsNotNull(result.ProductListings);
                Assert.IsNull(result.ShippingRegions);
                Assert.That(result.ProductListings.Count, Is.EqualTo(1));
                Assert.That(result.ProductListings[0].Price, Is.EqualTo(expectedShop.ProductListings[1].Price));
            }
        }

        [Test]
        public async Task IncludeWillFilterWithOrderBy()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Ordering = new List<OrderBy>
                            {
                                new OrderBy<ShopProductListing>
                                {
                                    PathExpression = x => x.AnotherValue,
                                    Order = Ordering.DESC
                                }
                            }
                        },

                    }
                }
            };


            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();
            Assert.That(results.Count, Is.EqualTo(3));

            foreach (var actualShop in results)
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == actualShop.Name);
                var expectedListings = expectedShop.ProductListings.OrderByDescending(x => x.AnotherValue).ToList();

                for(var i=0; i<actualShop.ProductListings.Count; i++)
                {
                    Assert.That(actualShop.ProductListings[i].AnotherValue, Is.EqualTo(expectedListings[i].AnotherValue));
                }

            }
        }

        [Test]
        public async Task IncludeWillFilterWithOrderByAndSkipAndTake()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Skip = 1,
                            Take = 1,
                            Ordering = new List<OrderBy>
                            {
                                new OrderBy<ShopProductListing>
                                {
                                    PathExpression = x => x.AnotherValue,
                                    Order = Ordering.DESC
                                }
                            }
                        },

                    }
                }
            };


            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();
            Assert.That(results.Count, Is.EqualTo(3));

            foreach (var actualShop in results)
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == actualShop.Name);
                var expectedListings = expectedShop.ProductListings.OrderByDescending(x => x.AnotherValue).Skip(1).Take(1).ToList();

                Assert.That(actualShop.ProductListings.Count, Is.EqualTo(1));
                Assert.That(actualShop.ProductListings[0].AnotherValue, Is.EqualTo(expectedListings[0].AnotherValue));
            }
        }

        [Test]
        public async Task IncludeWillFilterWithWhere()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            WhereClause = new RuleSet
                            {
                                Rules = new List<Rule>
                                {
                                    new Rule<ShopProductListing>
                                    {
                                        PathExpression = x => x.Price,
                                        ComparisonOperator = ComparisonOperators.Equal,
                                        Value = 10m
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            Assert.That(results.Count, Is.EqualTo(3));

            foreach(var result in results)
            {
                Assert.IsNotNull(result.ProductListings);
                Assert.IsNull(result.ShippingRegions);
            }

            var actualShop = results.Single(x => x.Name == ShopNames.Everythings10Shop);
            var expectedShop = TestData.Shops.Single(x => x.Name == ShopNames.Everythings10Shop);
            Assert.That(actualShop.ProductListings.Count(), Is.EqualTo(expectedShop.ProductListings.Count));
        }

        [Test]
        public async Task IncludeWillFilterWithWhereOrderBySkipAndTake()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<Shop>
                    {
                        PathExpression = x => x.ProductListings,
                        Filter = new Filter
                        {
                            Skip = 1,
                            Take = 1,
                            WhereClause = new RuleSet
                            {
                               Rules = new List<Rule>
                               {
                                   new Rule<ShopProductListing>
                                   {
                                       PathExpression = x => x.Product.Name,
                                       ComparisonOperator = ComparisonOperators.Equal,
                                       Value = ProductNames.CatDispenser
                                   }
                               }  
                            },
                            Ordering = new List<OrderBy>
                            {
                                new OrderBy<ShopProductListing>
                                {
                                    PathExpression = x => x.AnotherValue,
                                    Order = Ordering.DESC
                                }
                            }
                        },

                    }
                }
            };


            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();
            Assert.That(results.Count, Is.EqualTo(3));

            foreach (var actualShop in results)
            {
                var expectedShop = TestData.Shops.Single(x => x.Name == actualShop.Name);
                var expectedListings = expectedShop.ProductListings.Where(x => x.Product.Name == ProductNames.CatDispenser).OrderByDescending(x => x.AnotherValue).Skip(1).Take(1).ToList();

                Assert.That(actualShop.ProductListings.Count, Is.EqualTo(expectedListings.Count));

                if(actualShop.ProductListings.Count > 0)
                    Assert.That(actualShop.ProductListings[0].AnotherValue, Is.EqualTo(expectedListings[0].AnotherValue));
            }
        }

        [Test]
        public async Task InnerIncludeWillApplySkipTakeWhereAndOrderBy()
        {
            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShippingRegion>
                    {
                        PathExpression = x => x.Shops,
                        Filter = new Filter
                        {
                            Includes = new List<Include>
                            {
                                new Include<Shop>
                                {
                                    PathExpression = x => x.ProductListings,
                                    Filter = new Filter
                                    {
                                        Skip = 1,
                                        Take = 1,
                                        WhereClause = new RuleSet
                                        {
                                           Rules = new List<Rule>
                                           {
                                               new Rule<ShopProductListing>
                                               {
                                                   PathExpression = x => x.Product.Name,
                                                   ComparisonOperator = ComparisonOperators.Equal,
                                                   Value = ProductNames.CatDispenser
                                               }
                                           }
                                        },
                                        Ordering = new List<OrderBy>
                                        {
                                            new OrderBy<ShopProductListing>
                                            {
                                                PathExpression = x => x.AnotherValue,
                                                Order = Ordering.DESC
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };


            var query = QueryBuilder.BuildQuery<ShippingRegion>(filter);
            var results = await query(DbContext.ShippingRegions).ToListAsync();

            Assert.That(results.Count, Is.EqualTo(2));

            foreach(var region in results)
            {
                var expectedRegion = TestData.ShippingRegions.Single(x => x.Id == region.Id);
                var expectedShops = TestData.Shops.Where(x => x.ShippingRegions.Any(y => y.Id == expectedRegion.Id));
                Assert.That(region.Shops.Count, Is.EqualTo(expectedShops.Count()));

                foreach(var shop in region.Shops)
                {
                    var expectedShop = expectedShops.Single(x => x.Id == shop.Id);
                    var expectedListings = expectedShop.ProductListings.Where(x => x.Product.Name == ProductNames.CatDispenser).OrderByDescending(x => x.AnotherValue).Skip(1).Take(1);

                    Assert.That(shop.ProductListings.Count, Is.EqualTo(expectedListings.Count()));
                }
            }
            
        }
    }
}
