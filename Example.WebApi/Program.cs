using EfCore.Filtering.Mvc;
using EfCore.Filtering.ServiceBuilder;
using ExampleApi.Data;
using ExampleApi.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ShoppingDbContext>((opts) =>
{
    ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    opts.UseLoggerFactory(loggerFactory);
    opts.UseInMemoryDatabase("Shopping");

});

builder.Services.AddControllers(opts =>
{
    opts.ModelBinderProviders.Insert(0, new FilterModelBinderProvider());
}).AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

//builder.Services.AddControllers(opts =>
//{
//    opts.ModelBinderProviders.Insert(0, new FilterModelBinderProvider());
//});

builder.Services.AddQueryBuilder();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(g =>
{
    g.SwaggerDoc("v1", new OpenApiInfo { Title = "Example", Version = "V1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();



app.SeedData().Run();
