using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestSupport.EfHelpers;

namespace Tests.EfCore.Filtering.TestDb
{
    public static class InMemoryTestDbContextBuilder
    {
        public static TestDbContext CreateContext()
        {
            var options = SqliteInMemory.CreateOptions<TestDbContext>(builder =>
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });

                builder.UseLoggerFactory(loggerFactory);
            });

            //            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            //    {
            //        builder.AddConsole();
            //    });

            //var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            //optionsBuilder.UseSqlServer("Server=ANDY-XPS15\\MYSQLSERVER;Database=Test;Trusted_Connection=True;");
            //optionsBuilder.UseLoggerFactory(loggerFactory);          

            var context = new TestDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
