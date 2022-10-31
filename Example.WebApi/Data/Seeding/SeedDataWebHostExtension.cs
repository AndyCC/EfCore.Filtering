using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleApi.Data.Seeding
{
    public static class SeedDataWebHostExtension
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ShoppingDbContext>();
                context.SeedDataAndClearChangesAsync().Wait();
            }

            return host;
        }
    }
}
