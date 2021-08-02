# EfCore.Filtering
Builds Entity Framework Core queries from a common filter.

## Overview

Specify Skip, Take, Ordering, complex Where clauses and Includes and pass these through to an api endpoint to construct and execute queries against a database using Entity Framework Core 5.

Api endpoints can be set up to recieve the filter via a GET endpoint using pure query strings or where the query string contains a filter key where the value is json. Alternatively a POST endpoint can be setup to receieve JSON. Both examples are shown in in the ExampleApi project

## Setup

In `Startup.cs` in the `ConfigureServices` method add the model binder used to bind to the Filter. This requires the JsonSerializer to have the `ReferenceHandler` set to preserve.

```c#
services.AddControllers(opts =>
      {
           opts.ModelBinderProviders.Insert(0, new FilterModelBinderProvider());
      }).AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
 ```
 
 Also add the QueryBuilder to the service collection. The default method below will create an instance of the QueryBuilder and add it as a singleton. This method has various options to override the default build and setup of the QueryBuilder.
 
 ```c#
services.AddQueryBuilder();
 ```
 
 Inject `IQueryBuilder` into the required controller and add methods to send a filter to the api and get a response such as:
 
 ```c#

private MyDbContext _context;
 
[HttpGet]
public async Task<IActionResult> Get([FromQuery] Filter<Product> filter)
{
    if (!ModelState.IsValid)
         return BadRequest(ModelState);

    IList<Product> results;

    if (filter != null)
    {
         var query = QueryBuilder.BuildQuery<Product>(filter);
         results = await query(_context.Products).AsNoTracking().ToListAsync();
    }
    else
        results = await _context.Products.AsNoTracking().ToListAsync();

    if (results.Count == 0)
        return NotFound();

   return Ok(results);
}

 ```
 
You MUST use Filter<T> in the query endpoints, where T is the entity to which the filter will be applied. This is to assist with the model binding of the filter.
If the model binding fails it will add errors to the ModelState.
 
In the ExampleApi controller there is a handy base class. 
 
### The Filter
      
When using json with a GET method then the parameter must be called `filter`.

**Paths** are navigation paths which state how to reach a property which is to be ordered or queries on or included. Given a class `Product` if I wanted to say order on Id the path would just be `Id`. The Filter<T> definition in the filter method defines the source entity type and the navigation paths proceed from there. If `Product` has a series of `ShopProductListings` defining which `Shop`s the product was sold from and I wanted to order by the shop name when querying `Product` then the path would be `ShopProductListings.Shop.Name`  
 
#### Long Form Json & Filter Structure

Long form json isn't suitable for querystrings, due to size limitations of querystrings. 

A query could look something like, only the required parts of the filter need to be supplied:
      
```json
{
      "Skip": 1,
      "Take": 2,
      "WhereClause": {
          "LogicalOperator": "AND",
          "Rules": [{
                 "Path": "Product.Name",
                 "ComparisonOperator": "eq",
                 "Value": "Cat Dispenser"
             }, {
                 "Path": "Shop.Id",
                 "ComparisonOperator": "gte",
                 "Value": 1
             }],
           "RuleSets": null
      },
      "Ordering": [{
            "Path": "Id",
            "Order": "asc"
      }],
      "Includes": [{
          "Path": "Product",
          "Filter": null
      },{
          "Path": "Shop",
          "Filter": null
      }]
}      
```

*Skip* and *Take* relate to the Skip and Take methods in EF Core.

The *WhereClause* is a Ruleset. A RuleSet contains a `LogicalOperator` which can be `AND` or `OR` and is used to combine the rules and sub-rulesets together. An array of `Rules` which define which properties to query against (`Path`), how (`ComparisonOperator`) and against what value (`Value`), and an array of `RuleSets`. Each `RuleSet` in the array of `RuleSets` takes the same form as the WhereClause, which is a RuleSet. 
      
The nesting of rulesets allow for complex combinations such as `A AND B AND (C OR D)` or `A OR B OR (C AND D)` or `(A OR B) AND (C OR D)`

A where clause must have at least 1 rule or a ruleset defined. See the Comparison Rules section below for more specifics on the rules that are available.
      
*Ordering* contains a list of order bys. These are defined by a `Path` and a direction (`Order`). The `Order` can be asc or desc. The order bys are added to the query in the same order that they appear in the Filter json.
      
*Includes* these are array of Paths and Filters which define the required includes. If adding an include only the `Path` to the required association is required, a `Filter` can also be added, this filter takes the same form as the Filter defined above.
      
#### Short Form Json

### Pure Query String
   
### Comparison Rules

      
## Extensibility

### Rules

### Parts

### Misc

## Projects

### EfCore.Filtering.Mvc
This project contains model binding to translate a pure query string or query string of json or body of json into a Filter object 

### EfCore.Filtering.Client
This project contains the main objects used to defined the filter

### EfCore.Filtering
This is the main project that contains the logic to build the queries
