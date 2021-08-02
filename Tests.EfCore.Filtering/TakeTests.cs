using EfCore.Filtering.Client;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Tests.EfCore.Filtering.TestDb;
using Tests.EfCore.Filtering.TestDb.Models;

namespace Tests.EfCore.Filtering
{
    public class TakeTests : QueryBuilderTestBase
    {
        [Test]
        public async Task WithNoTakeReturnsAllItems()
        {
            var filter = new Filter();

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count, Is.EqualTo(TestData.Products.Count()));
        }

        [Test]
        public async Task WithTakeOneReturnsFirstItem()
        {
            var filter = new Filter
            {
                Take = 1,
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Id, Is.EqualTo(1));
        }

        [Test]
        public async Task WithTakeOneSkipOneReturnsSecondItem()
        {
            var filter = new Filter
            {
                Skip = 1,
                Take = 1,
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Id, Is.EqualTo(2));
        }
    }
}
