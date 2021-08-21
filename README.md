# EfCore.Filtering
Builds Entity Framework Core queries from a common filter.

## Overview

Specify Skip, Take, Ordering, complex Where clauses and Includes and pass these through to an api endpoint to construct and execute queries against a database using Entity Framework Core.

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
 
You MUST use Filter\<T\> in the query endpoints, where T is the entity to which the filter will be applied. This is to assist with the model binding of the filter.
If the model binding fails it will add errors to the ModelState.
 
In the ExampleApi controller there is a handy base class. 
 
### The Filter
      
When using json with a GET method then the parameter must be called `filter`.

**Paths** are navigation paths which state how to reach a property which is to be ordered or queries on or included. Given a class `Product` if I wanted to say order on Id the path would just be `Id`. The Filter<T> definition in the filter method defines the source entity type and the navigation paths proceed from there. If `Product` has a series of `ShopProductListings` defining which `Shop`s the product was sold from and I wanted to order by the shop name when querying `Product` then the path would be `ShopProductListings.Shop.Name`  
 
#### Long Form Json & Filter Structure

Long form json isn't suitable for querystrings, due to size limitations of querystrings. 

A filter could look something like, only the required parts of the filter need to be supplied:
      
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

The short form json is for use in query strings to reduce the size of the query string to acceptable lengths. Therefore remove any whitespace from the json, if using this method.
      
The short form json is basically the same structure as the long form json, but the property names are only 1 letter long on the filter object. So the property `Path` becomes `P`. In addition to this the Ordering becomes a list of strings where `My.Path` will order by `My.Path` in ascending order and `-My.Path` will order by descending. The `LogicalOperator` in the where clause can be sepecied with `O` for `OR` or `A` for `AND`.
      
And example of short form json:
      
This json is run against `Shop` in the ExampleApi. 
This json will create a query against shop:
  * Order by Id ascending
  * Select shops where the name is equal to "Everythings 10 Shop" OR ( the shop has a ProductListing with a price greater the 14 AND the Shop has a Shipping Region of EU or UK )
  * The query will include ProductListings taking the second ProductListing returned, due to the Skip 1, Take 1 on the include.    
      
```json
{
  "O": [
    "Id"
  ],
  "W": {
    "L": "O",
    "R": [
      {
        "P": "Name",
        "C": "eq",
        "V": "Everythings 10 Shop"
      }
    ],
    "S": [
      {
        "L": "A",
        "R": [
          {
            "P": "ProductListings.Price",
            "C": "gt",
            "V": 14
          },
          {
            "P": "ShippingRegions.Region",
            "C": "in",
            "V": [
              "UK",
              "EU"
            ]
          }
        ]
      }
    ]
  },
  "I": [
    {
      "P": "ProductListings",
      "F": {
        "T": 1,
        "S": 1
      }
    }
  ]
}          
```

This could be compressed to the following url
      
`GET` `myapi/Shop?filter={"O":["Id"],"W":{"L":"O","R":[{"P":"Name","C":"eq","V":"Everythings 10 Shop"}],"S":[{"L":"A","R":[{"P":"ProductListings.Price","C":"gt","V":14},{"P":"ShippingRegions.Region","C":"in","V":["UK","EU"]}]}]},"I":[{"P":"ProductListings", "F":{"T":1,"S":1}}]}`
      
#### Pure Query String
      
The pure query string is based on the short form json, in order to keep the query string small. So the same rule around property names and ordering and logical operators apply. In addition an array of values in a rule will start with a `[` and end with a `]`. 
 
The short form json example from above, written as a query string would be as follows. The example is split across multiple lines for readability.
 
```
myapi/Shop?O[0]=Id
      &W.L=O
      &W.R[0].P=Name
      &W.R[0].C=eq
      &W.R[0].V=Everythings%2010%20Shop
      &W.S[0].L=A
      &W.S[0].R[0].P=ProductListings.Price
      &W.S[0].R[0].C=gt
      &W.S[0].R[0].V=14
      &W.S[0].R[1].P=ShippingRegions.Region
      &W.S[0].R[1].C=in
      &W.S[0].R[1].V=[UK,EU]
      &I[0].P=ProductListings
      &I[0].F.T=1
      &I[0].F.S=1 
```
   
### Default Rules

The following comparison rules are provided. Comparison operators are not case sensitive

Comparison Operator | Definition
--------------------|------------
in | equivelant to SQL IN statement, expects rule value to be an array.
like | equivalent to SQL LIKE statement, expects rule value to be a string. Any wildcards must be supplied in the rule's value.
eq | SQL = 
equals | SQL =
ne | SQL <> 
notEqual | SQL <>
ge | SQL > 
greaterThan | SQL >
gte | SQL >= 
greaterThanOrEqual | SQL >=
lt | SQL < 
lessThan | SQL <
lte | SQL <= 
lessThanOrEqual | SQL <=
      
An example of a long and short formed json rule:
```json
{
   "Path": "Product.Name",
   "ComparisonOperator": "eq",
   "Value": "Cat Dispenser"
}
```
```json
{
   "P": "Product.Name",
   "C": "eq",
   "V": "Cat Dispenser"
}
```

      
## Extensibility

### Rule Expression Builders
A rule expression builder tells the QueryBuilder when and how to interpret a given rule type. To do this add a new rule type a rule builder must be implemented `IRuleExpressionBuilder`
      
IRuleExpressionBuilder has two methods that need to be implemented:
 
 ```c#
    public bool CanInterpretRule(Rule rule);
    public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context);
 ```
      
 Classes used by these methods are the Rule and RuleBuilderContext.
 Rule defines a rule that has been passed into the QueryBuilder:
      
```c#
/// <summary>
/// A filtering rule
/// </summary>
public class Rule
{
    /// <summary>
    /// Object path from source entity to reach the property to evaluate
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Comparison operator for the rule
    /// </summary>
    public string ComparisonOperator { get; set; }

    /// <summary>
    /// Value to evaluate against
    /// </summary>
    public Object Value { get; set; }
}
```

The RuleBuilderContext contains some useful items for use in building the rule. It is defined as:

```c#
/// <summary>
/// Context used by a rule expression builder
/// </summary>
public class RuleBuilderContext
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="parameterExpression">parameter used to access for the source entity</param>
    /// <param name="targetPropertyType">Type of the target property</param>
    internal RuleBuilderContext(ParameterExpression parameterExpression, Type targetPropertyType)
    {
        ParameterExpression = parameterExpression;
        TargetPropertyType = targetPropertyType;
    }

    /// <summary>
    /// parameter used to access for the source entity
    /// </summary>
    public ParameterExpression ParameterExpression { get; private set; }

    /// <summary>
    /// Type of the target property
    /// </summary>
    public Type TargetPropertyType { get; private set; }
}
```

The ParameterExpression an expression tree parameter which represents the source entity. The path in the rule passed into this method will exist on this entity.
The TargetPropertyType is the type of the very last property in the path.
      
**CanInterpretRule**

This method will evaluate the `Rule` object and determine if this builder can create an expression. If multiple builders can interpret the rule then the rule won't be interpreted, and a `RuleExpressionBuilderNotFoundException` will be thrown.

**BuildRuleExpression**
This method takes the rule and the context and creates an expression. The best way to show this is via an example, below is the `SimpleComparisonRuleBuilder`.

In this example the method:
1) Converts the path in the rule to be an expression
2) Creates a constant expression value to compare against
3) Evaluates the comparson operator and returns an expression to perform the required comparison.

```c#
public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context)
{
    if (context == null)
        throw new ArgumentNullException(nameof(context));

    var propertyPathExpression = PropertyPath.AsPropertyExpression(rule.Path, context.ParameterExpression);
    var constantExpression = Expression.Constant(rule.Value);

    return rule.ComparisonOperator.ToLower() switch
    {
        "equal" or "eq" => Expression.Equal(propertyPathExpression, constantExpression),
        "notequal" or "ne" => Expression.NotEqual(propertyPathExpression, constantExpression),
        "greaterthan" or "gt" => Expression.GreaterThan(propertyPathExpression, constantExpression),
        "greaterthanorequal" or "gte" => Expression.GreaterThanOrEqual(propertyPathExpression, constantExpression),
        "lessthan" or "lt" => Expression.LessThan(propertyPathExpression, constantExpression),
        "lessthanorequal" or "lte" => Expression.LessThanOrEqual(propertyPathExpression, constantExpression),
        _ => throw new NotImplementedException($"Comparison operator {rule.ComparisonOperator} not implemented"),
   };
}
```

To add the rule to the query builder, it needs to be added to the list of `RuleExpressionBuilder`s in `StartUp.cs`. In this example the default rule expression builders are added before the new builder. Currently the default rule expression builders are listed above in the default rules and are implemented by 3 classes: `InRuleBuilder`, `LikeRuleBuilder` and `SimpleComparisonRuleBuilder`

```c#
services.AddQueryBuilder(opts =>
{
    opts.UseDefault(ruleExpressionOptions: ruleOpts =>
    {
       ruleOpts.UseDefault()
                .UseRuleExpressionBuilder(typeof(MyNewRuleExpressionVBuilder));
    });
})
```

### Builder Parts
Builder parts implement a _part_ of a query. For example the current builder parts include `skip`, `take`, `include`, `where` and `orderby`. It is possible to add new parts or replace existing ones.

There are 2 interfaces to consider implementing when creating a part. `IBuilderPart` and `IIncludeFilteringPart`, the two interfaces appear very similar however define where the part can be used. `IBuilderPart` is used in the main query and `IIncludeFilteringPart` states how to create the part when used within the `Include` part as in EF.Core Includes can contain their own queries. In the code base you'll often see these implemented together as the logic to create the part is the same with the exception that the main query construction requires expressions to be built that use IQueryable and the _include_ query requries expressions to be built that use IEnumerable.

The interfaces are defined as:

```c#
/// <summary>
///  Builds an expression tree to represent logic to be built by the QueryBuilder
/// </summary>
public interface IBuilderPart
{
    /// <summary>
    /// determines when part will be executed. Lower numbers execute first
    /// </summary>
    public int ExecutionOrder { get; set; }

    /// <summary>
    /// Builds part of an expression
    /// </summary>
    /// <param name="context">BuilderContext</param>
    /// <returns>updated expression</returns>
    public Expression BuildExpression(BuilderContext context);
}
```

```c#
/// <summary>
/// Builds an expression tree to represent logic to be included in the QueryBuilder's Include part. 
/// </summary>
public interface IIncludeFilteringPart
{
    /// <summary>
    /// determines when part will be executed. Lower numbers execute first
    /// </summary>
    public int ExecutionOrder { get; set; }

    /// <summary>
    /// Builds part of an expression for an include's filter
    /// </summary>
    /// <param name="context">BuilderContext</param>
    /// <returns>updated expression</returns>
    public Expression BuildIncludeExpression(BuilderContext context);
}
```

**ExecutionOrder**

This property defines the order in which the parts are to be executed when constructing the expression tree which will create the query. This value can be negative.

Currently this is set as follows:

Part | Order
--------|------
Where | 0
Include | 500
OrderBy | 1000
Skip | 1500
Take | 2000

**BuildIncludeExpression**

This method takes a BuilderContext and returns the expression that needs to be added to the expression tree which will create the query. The BuilderContext is defined as:

```c#
  /// <summary>
    /// Context used for building the query
    /// </summary>
    public class BuilderContext
    {
        /// <summary>
        /// The expression that has been built so far
        /// </summary>        
        public Expression CurrentExpression { get; set; }

        /// <summary>
        /// The filter being applied
        /// </summary>
        public Filter Filter { get;  set; }

        /// <summary>
        /// Source Entity Type for use with the Paths in the filter
        /// </summary>
        public Type SourceEntityType { get; set; }

        /// <summary>
        /// PathWalker containing details of all the paths in the filter
        /// </summary>
        public PathWalker PathWalker { get; internal set; }

        /// <summary>
        /// Determines if the context is valid or not
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {
            return CurrentExpression != null &&
                Filter != null &&
                SourceEntityType != null;
        }
    }
```

The new expression must be build on top of or include the `CurrentExpression`
The `Filter` is the full filter that has been passed into the QueryBuilder
The `SourceEntityType` is the entity type the query is to be built against, and from which all rule paths originate. In the `IIncludeFilteringPart` the ``SourceEntityType` is the entity type that is included by an `Include`.
The `PathWalker` contains all the paths declared in the filter, it validates them all and provides useful functions to access details about the paths.

To add custom parts to the QueryBuilder then the whole QueryBuilder setup needs to be defined in `Startup.cs`. If using the default WherePart then a RuleSetExpressionBuilder also needs to be defined. To use the default implementation use `UseDefaultRuleSetExpressionBuilder` and `UseRuleBuilders`:

```c#
services.AddQueryBuilder(opts =>
{
   opts.UseDefaultWherePart()
       .UseDefaultIncludePart()
       .UseDefaultOrderBy()
       .UseDefaultSkip()
       .UseDefaultTake()
       .UsePart(typeof(MyPart))
       .UseDefaultRuleSetExpressionBuilder()
       .UseRuleBuilders();
});
```


### RuleSetExpressionBuilder
The `RuleSetExpressionBuilder` used by the default where clause can be replaced by implementing `IRuleSetExpressionBuilder`.

`Startup.cs` must also by updated to specify the new RuleSetExpressionBuilder. 

```c#
services.AddQueryBuilder(opts =>
{
    opts.UseDefaultWherePart()
        .UseDefaultIncludePart()
        .UseDefaultOrderBy()
        .UseDefaultSkip()
        .UseDefaultTake()
        .UseRuleSetExpressionBuilder(typeof(MyRuleSetExpressionBuilder))
        .UseRuleBuilders();
});
```

`UseRuleBuilders()` will only be required if your implementation of `IRuleSetExpressionBuilder` requires them.
      
## Projects

### EfCore.Filtering.Mvc
This project contains model binding to translate a pure query string or query string of json or body of json into a Filter object 

### EfCore.Filtering.Client
This project contains the main objects used to defined the filter

### EfCore.Filtering
This is the main project that contains the logic to build the queries
