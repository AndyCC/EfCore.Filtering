using EfCore.Filtering;
using ExampleApi.Data;
using ExampleApi.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingRegionController : CRUDQBaseController<ShippingRegion, int>
    {
        public ShippingRegionController(ShoppingDbContext dbContext,
                              IQueryBuilder queryBuilder)
            : base(dbContext,
                   queryBuilder,
                   (ctx) => ctx.ShippingRegions,
                   (e) => e.Id)

        {
        }
    }
}
