using EfCore.Filtering;
using ExampleApi.Data;
using ExampleApi.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CRUDQBaseController<Product, int>
    {
        public ProductController(ShoppingDbContext dbContext,
                              IQueryBuilder queryBuilder)
            : base(dbContext,
                   queryBuilder,
                   (ctx) => ctx.Products,
                   (e) => e.Id)

        {
        }
    }
}
