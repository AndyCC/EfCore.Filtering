using EfCore.Filtering;
using NUnit.Framework;
using System.Threading.Tasks;
using Tests.EfCore.Filtering.TestDb;

namespace Tests.EfCore.Filtering
{
    public abstract class QueryBuilderTestBase
    {
        protected TestDbContext DbContext { get; private set; }
        protected QueryBuilder QueryBuilder { get; private set; }

        [SetUp]
        public async Task Setup()
        {
            DbContext = InMemoryTestDbContextBuilder.CreateContext();
            await DbContext.SeedDataAndClearChangesAsync();

            QueryBuilder = QueryBuilder.DefaultBuilder;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (DbContext != null)
                await DbContext.DisposeAsync();
        }
    }
}
