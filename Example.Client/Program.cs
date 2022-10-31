// See https://aka.ms/new-console-template for more information
using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using Example.Client;
using ExampleApi.Data.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var filter = new Filter<ShopProductListing>
{
    Take = 2,
    WhereClause = new RuleSet
    {
        LogicalOperator = LogicalOperators.AND,
        Rules = new List<Rule>
        {
            new Rule<ShopProductListing>
            {
                PathExpression = l => l.Product.Name,
                ComparisonOperator = ComparisonOperators.Equal,
                Value = "Cat Dispenser"
            },
            new Rule<ShopProductListing>
            {
                PathExpression = l => l.Shop.Id,
                ComparisonOperator = ComparisonOperators.GreaterThan,
                Value = 1
            }
        }
    }, 
    Ordering = new List<OrderBy>
    {
        new OrderBy<ShopProductListing>
        {
            PathExpression = l => l.Id,
            Order = Ordering.ASC
        }
    },
    Includes = new List<Include>
    {
        new Include<ShopProductListing>
        {
            PathExpression = l => l.Product
        },
        new Include<ShopProductListing>
        {
            PathExpression = l => l.Shop
        }
    }
};


var jsonOptions = new JsonSerializerOptions();
jsonOptions.AddFilterConvertors();

HttpResponseMessage? response = null;

try
{
    using var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7289/") };

    response = await httpClient.PostAsJsonAsync("api/ShopProductListing/query", filter, jsonOptions);

    response.EnsureSuccessStatusCode();

    Console.WriteLine($"StatusCode: {response.StatusCode}");

    var jsonDeSerializerOptions = new JsonSerializerOptions();
    jsonDeSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    jsonDeSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    var results = await response.Content.ReadFromJsonAsync<List<ShopProductListing>>(jsonDeSerializerOptions);

    if (results == null)
        throw new Exception("null results");


    foreach (var listing in results)
    {
        Console.WriteLine($"{listing.Id}: Product - {listing.Product.Name}, Shop - {listing.Shop.Name}, Price - {listing.Price:C}");
    }
}
catch(Exception ex)
{
    if (response != null)
    {
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine(content);
        Console.WriteLine();
    }

    Console.WriteLine(ex);
}

Console.WriteLine("Press Any Key To Exit");
Console.ReadKey();