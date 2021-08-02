using EfCore.Filtering;
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
    public class SkipTests : QueryBuilderTestBase
    {       
        [Test]
        public async Task WithNoSkipReturnsAllItems()
        {
            var filter = new Filter();

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count, Is.EqualTo(TestData.Products.Count()));
        }

        [Test]
        public async Task WithSkipOneIgnoresFirstItem()
        {
            var filter = new Filter
            {
                Skip = 1,
            };

            var query = QueryBuilder.BuildQuery<Product>(filter);

            var results = await query(DbContext.Products).ToListAsync();

            Assert.IsNotNull(results);
            Assert.IsTrue(results.GetType().IsAssignableTo(typeof(IEnumerable)));
            Assert.That(results.Count, Is.EqualTo(TestData.Products.Count() - 1));
            Assert.IsFalse(results.Any(x => x.Id == 1));
        }
    }
}