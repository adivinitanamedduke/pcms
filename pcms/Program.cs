using API.Middleware;
using Asp.Versioning;
using Core.Repository;
using Core.Repository.Data;
using Data;
using Data.Entities;
using Data.EntityFramework.Repositories;
using Data.Repositories;
using Data.Utilities;
using Domain.Services;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
 {
     options.GroupNameFormat = "'v'VVV";// Format:e.g v1
     options.SubstituteApiVersionInUrl = true;
 });


// Add services to the container.
builder.Services.AddDbContext<InMemoryDbContext>(options => options.UseInMemoryDatabase("PcmsDb")); 
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
// Pure In-Memory Repository for Categories
builder.Services.AddSingleton<InMemoryCategoryRepository>();
builder.Services.AddSingleton<IRepository<Category, int>>(sp =>
    sp.GetRequiredService<InMemoryCategoryRepository>());

builder.Services.AddSingleton<IReadRepository<Category, int>>(sp =>
    sp.GetRequiredService<InMemoryCategoryRepository>());
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;//Ensure all endpoints are in lowercase
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to find endpoints

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PCMS (Product Catalog Management System) API",
        Version = "v1",
        Description = "API for Products (EF In-Memory) and Categories (Dictionary-based)"
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("https://localhost:4200");
    });
});

var app = builder.Build();
// Seed Categories
using (var scope = app.Services.CreateScope())
{
    await CategorySeeder.SeedCategories(scope.ServiceProvider);
}
//Custom middleware for global exception handling
app.UseGlobalExceptionHandler();
// Custom middleware runs pattern matching
app.UseMiddleware<ProductValidationMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PCMS API v1");
        options.RoutePrefix = string.Empty; // Serves Swagger UI at the app's root (localhost:port/)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.Run();

