using EfCore.Filtering;
using ExampleApi.Data;
using ExampleApi.Data.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : CRUDQBaseController<Shop, int>
    {
        public ShopController(ShoppingDbContext dbContext,
                              IQueryBuilder queryBuilder)
            : base(dbContext,
                   queryBuilder,
                   (ctx) => ctx.Shops,
                   (e) => e.Id)

        {
        }
    }
}
