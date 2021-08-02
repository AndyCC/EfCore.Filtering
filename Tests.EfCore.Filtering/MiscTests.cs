using EfCore.Filtering;
using EfCore.Filtering.Client;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.EfCore.Filtering.TestDb.Models;

namespace Tests.EfCore.Filtering
{
    public class MiscTests : QueryBuilderTestBase
    {
        [Test]
        public void AnInvalidPathInWhereWillThrowAnException()
        {
            const string path = "Shop.ShopId";

            var filter = new Filter
            {
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule<ShopProductListing>
                        {
                            Path = path,
                            ComparisonOperator = ComparisonOperators.Equal,
                            Value = 1234
                        }
                    }
                }
            };

            var exception = Assert.Throws<InvalidPathsException>(() => QueryBuilder.BuildQuery<ShopProductListing>(filter));

            var entityType = typeof(ShopProductListing);
            Assert.IsTrue(exception.InvalidPaths.ContainsKey(entityType));
            Assert.IsTrue(exception.InvalidPaths[entityType].Contains(path));
        }

        [Test]
        public void AnInvalidPathInOrderByWillThrowAnException()
        {
            const string path = "Shop.ShopId";

            var filter = new Filter
            {
                Ordering = new List<OrderBy>
                {
                    new OrderBy<ShopProductListing>
                    {
                        Path = path,
                        Order = Ordering.ASC,
                    }
                }
            };

            var exception = Assert.Throws<InvalidPathsException>(() => QueryBuilder.BuildQuery<ShopProductListing>(filter));

            var entityType = typeof(ShopProductListing);
            Assert.IsTrue(exception.InvalidPaths.ContainsKey(entityType));
            Assert.IsTrue(exception.InvalidPaths[entityType].Contains(path));
        }

        [Test]
        public void AnInvalidPathInIncludesWillThrowAnException()
        {
            const string path = "Shop.ShopId";

            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShopProductListing>
                    {
                        Path = path,
                    }
                }
            };

            var exception = Assert.Throws<InvalidPathsException>(() => QueryBuilder.BuildQuery<ShopProductListing>(filter));

            var entityType = typeof(ShopProductListing);
            Assert.IsTrue(exception.InvalidPaths.ContainsKey(entityType));
            Assert.IsTrue(exception.InvalidPaths[entityType].Contains(path));
        }

        [Test]
        public void AnInvalidPathInInnerIncludesWhereWillThrowAnException()
        {
            const string path = "Shop.Location.Id";

            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShopProductListing>
                    {
                        PathExpression = x => x.Shop,
                        Filter = new Filter
                        {
                            WhereClause = new RuleSet
                            {
                                Rules = new List<Rule>
                                {
                                    new Rule<Shop>
                                    {
                                        Path = path,
                                        ComparisonOperator = ComparisonOperators.Equal,
                                        Value = 1
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var exception = Assert.Throws<InvalidPathsException>(() => QueryBuilder.BuildQuery<ShopProductListing>(filter));

            var entityType = typeof(Shop);
            Assert.IsTrue(exception.InvalidPaths.ContainsKey(entityType));
            Assert.IsTrue(exception.InvalidPaths[entityType].Contains(path));
        }


        [Test]
        public void AnInvalidPathInInnerIncludesOrderByWillThrowAnException()
        {
            const string path = "Shop.Location.Id";

            var filter = new Filter
            {
                Includes = new List<Include>
                {
                    new Include<ShopProductListing>
                    {
                        PathExpression = x => x.Shop,
                        Filter = new Filter
                        {
                            Ordering = new List<OrderBy>
                            {
                                 new OrderBy<Shop>
                                 {
                                     Path = path,
                                     Order = Ordering.ASC,
                                 }
                            }
                        }
                    }
                }
            };

            var exception = Assert.Throws<InvalidPathsException>(() => QueryBuilder.BuildQuery<ShopProductListing>(filter));

            var entityType = typeof(Shop);
            Assert.IsTrue(exception.InvalidPaths.ContainsKey(entityType));
            Assert.IsTrue(exception.InvalidPaths[entityType].Contains(path));
        }
    }

}
