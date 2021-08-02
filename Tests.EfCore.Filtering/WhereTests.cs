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
    public class  WhereTests : QueryBuilderTestBase
    {
        [Test]
        public async Task AddsEqualsClause()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Name,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = ProductNames.CatDispenser,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo(ProductNames.CatDispenser));
        }

        [Test]
        public async Task AddsNotEqualClause()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Name,
                            ComparisonOperator = ComparisonOperators.NotEqual,
                            Value = ProductNames.CatDispenser,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(TestData.Products.Count() - 1));
            Assert.IsTrue(!results.Any(x => x.Name == ProductNames.CatDispenser));
        }

        [Test]
        public async Task AddsGreaterThanClause()
        {
            const int id = 5;

            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.GreaterThan,
                            Value = id,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            var expectedProducts = TestData.Products.Where(x => x.Id > id);

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(expectedProducts.Count()));
            Assert.IsTrue(results.All(x => x.Id > id));
        }

        [Test]
        public async Task AddsGreaterThanOrEqualClause()
        {
            const int id = 5;

            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.GreaterThanOrEqual,
                            Value = id,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            var expectedProducts = TestData.Products.Where(x => x.Id >= id);

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(expectedProducts.Count()));
            Assert.IsTrue(results.All(x => x.Id >= id));
        }

        [Test]
        public async Task AddsLessThanClause()
        {
            const int id = 5;

            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.LessThan,
                            Value = id,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            var expectedProducts = TestData.Products.Where(x => x.Id < id);

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(expectedProducts.Count()));
            Assert.IsTrue(results.All(x => x.Id < id));
        }

        [Test]
        public async Task AddsLessThanOrEqualClause()
        {
            const int id = 5;

            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.LessThanOrEqual,
                            Value = id,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            var expectedProducts = TestData.Products.Where(x => x.Id <= id);

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(expectedProducts.Count()));
            Assert.IsTrue(results.All(x => x.Id <= id));
        }

        [Test]
        public async Task AddsLikeClause()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Name,
                            ComparisonOperator = ComparisonOperators.Like,
                            Value = "%dis%",
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.IsTrue(results.Any(x => x.Name == ProductNames.CatDispenser));
            Assert.IsTrue(results.Any(x => x.Name == ProductNames.DogDispatcher));
        }

        [Test]
        public async Task AddsInClause()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.In,
                            Value = new int[] { 2, 3 },
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.IsTrue(results.Any(x => x.Id == 2));
            Assert.IsTrue(results.Any(x => x.Id == 3));
        }

        [Test]
        public async Task CombinesRulesWithAnd()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    LogicalOperator = LogicalOperators.AND,
                    Rules = new List<Rule>
                    {
                        new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Price,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = 10m,
                        },
                         new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Product.Name,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = ProductNames.CatDispenser
                        },
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);

            var results = await query(DbContext.ShopProductListings).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CombinesRulesWithOr()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    LogicalOperator = LogicalOperators.OR,
                    Rules = new List<Rule>
                    {
                        new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Product.Name,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = ProductNames.CarpetCleaner,
                        },
                         new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Product.Name,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = ProductNames.CatDispenser
                        },
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);

            var results = await query(DbContext.ShopProductListings).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task CombinesRulesWithAndThenInnerRulesWithOr()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    LogicalOperator = LogicalOperators.AND,
                    Rules = new List<Rule>
                    {
                        new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Price,
                            ComparisonOperator = ComparisonOperators.GreaterThanOrEqual,
                            Value = 9m,
                        },
                         new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Price,
                            ComparisonOperator = ComparisonOperators.LessThan,
                            Value = 11m,
                        },
                    },
                    RuleSets = new List<RuleSet>
                    {
                        new RuleSet
                        {
                            LogicalOperator = LogicalOperators.OR,
                            Rules = new List<Rule>
                            {
                                new Rule<ShopProductListing>
                                {
                                    PathExpression = x => x.Product.Name,
                                    ComparisonOperator = ComparisonOperators.Equal,
                                    Value = ProductNames.CatDispenser
                                },
                                new Rule<ShopProductListing>
                                {
                                    PathExpression = x => x.Product.Name,
                                    ComparisonOperator = ComparisonOperators.Equal,
                                    Value = ProductNames.DogDispatcher
                                }
                            }
                        }
                    },
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);

            var results = await query(DbContext.ShopProductListings).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task CombinesRulesWithOrThenInnerRulesWithAnd()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    LogicalOperator = LogicalOperators.OR,
                    Rules = new List<Rule>
                    {
                        new Rule<ShopProductListing>
                        {
                            PathExpression = x => x.Price,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = 10m,
                        },
                    },
                    RuleSets = new List<RuleSet>
                    {
                        new RuleSet
                        {
                            LogicalOperator = LogicalOperators.AND,
                            Rules = new List<Rule>
                            {
                                new Rule<ShopProductListing>
                                {
                                    PathExpression = x => x.Shop.Name,
                                    ComparisonOperator = ComparisonOperators.Equal,
                                    Value = ShopNames.AnimalShop
                                },
                                new Rule<ShopProductListing>
                                {
                                    PathExpression = x => x.Product.Name,
                                    ComparisonOperator = ComparisonOperators.Equal,
                                    Value = ProductNames.DogDispatcher
                                }
                            }
                        }
                    },
                }
            };

            var query = QueryBuilder.BuildQuery<ShopProductListing>(filter);

            var results = await query(DbContext.ShopProductListings).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(8));
        }

        [Test]
        public async Task AppliesSkipTakeAndOrderByWithWhere()
        {
            const int id = 5;

            var filter = new Filter
            {
                Skip = 1,
                Take = 2,
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<Product>
                        {
                            PathExpression = x => x.Id,
                            ComparisonOperator = ComparisonOperators.GreaterThanOrEqual,
                            Value = id,
                        }
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            var expectedProducts = TestData.Products.Where(x => x.Id >= id).Skip(1).Take(2);

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(expectedProducts.Count()));
            Assert.IsTrue(results.All(x => x.Id >= id));
        }

        [Test]
        public async Task NavigatesEnumerableOnWhere()
        {
            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    LogicalOperator = LogicalOperators.OR,
                    Rules = new List<Rule>
                    {
                        new Rule<Shop>
                        {
                            Path = "ProductListings.Price",
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = 10m,
                        },
                        new Rule<Shop>
                        {
                            Path = "ProductListings.Price",
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = 13m,
                        }
                    }
                },
                Ordering = new List<OrderBy>
                {
                    new OrderBy<Shop>
                    {
                        PathExpression = x => x.Name,
                        Order = Ordering.ASC
                    }
                }
            };

            var query = QueryBuilder.BuildQuery<Shop>(filter);
            var results = await query(DbContext.Shops).ToListAsync();

            Assert.IsNotNull(results);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].Name, Is.EqualTo(ShopNames.AnimalShop));
            Assert.That(results[1].Name, Is.EqualTo(ShopNames.Everythings10Shop));
        }
    }
}
