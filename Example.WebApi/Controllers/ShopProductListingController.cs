using EfCore.Filtering;
using ExampleApi.Data;
using ExampleApi.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopProductListingController : CRUDQBaseController<ShopProductListing, int>
    {
        public ShopProductListingController(ShoppingDbContext dbContext,
                              IQueryBuilder queryBuilder)
            : base(dbContext,
                   queryBuilder,
                   (ctx) => ctx.ShopProductListings,
                   (e) => e.Id)

        {
        }
    }
}
