using EfCore.Filtering;
using EfCore.Filtering.Client;
using ExampleApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    public abstract class CRUDQBaseController<TEntity, TId> : ControllerBase
        where TEntity : class
    {
        public CRUDQBaseController(ShoppingDbContext dbContext,
                                   IQueryBuilder queryBuilder,
                                   Func<ShoppingDbContext, IQueryable<TEntity>> dbContextProperty,
                                   Func<TEntity, TId> idFunc)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
            _dbContextProperty = dbContextProperty ?? throw new ArgumentNullException(nameof(dbContextProperty));
            _idFunc = idFunc ?? throw new ArgumentNullException(nameof(idFunc));
        }

        protected ShoppingDbContext DbContext { get; private set; }
        protected IQueryBuilder QueryBuilder { get; private set; }
        private readonly Func<ShoppingDbContext, IQueryable<TEntity>> _dbContextProperty;
        private readonly Func<TEntity, TId> _idFunc;

        // GET: api/<Controller>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Filter<TEntity> filter)
        {
            if (!ModelState.IsValid)            
                return BadRequest(ModelState);            

            IList<TEntity> results;

            if (filter != null)
            {
                var query = QueryBuilder.BuildQuery<TEntity>(filter);
                results = await query(_dbContextProperty(DbContext)).AsNoTracking().ToListAsync();
            }
            else
                results = await _dbContextProperty(DbContext).AsNoTracking().ToListAsync();

            if (results.Count == 0)
                return NotFound();

            return Ok(results);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(TId id)
        {
            var entity = await DbContext.FindAsync<TEntity>(id);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        // POST api/<Controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = _idFunc(entity) }, entity);
        }

        // PUT api/<Controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(TId id, [FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!id.Equals(_idFunc(entity)))
            {
                return BadRequest();
            }

            DbContext.Entry(entity).State = EntityState.Modified;

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(TId id)
        {
            var shop = await DbContext.FindAsync<TEntity>(id);

            if (shop == null)
                return NotFound();

            DbContext.Remove(shop);
            await DbContext.SaveChangesAsync();

            return Ok();
        }

        // POST api/<Controller>/query
        [HttpPost("query")]
        public async Task<IActionResult> Query(Filter<TEntity> filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var query = QueryBuilder.BuildQuery<TEntity>(filter);
            var results = await query(_dbContextProperty(DbContext)).AsNoTracking().ToListAsync();
            
            if (results.Count == 0)
                return NoContent();

            return Ok(results);
        }

        private bool EntityExists(TId id) => _dbContextProperty(DbContext).Any(e => _idFunc(e)!.Equals(id));
    }
}
