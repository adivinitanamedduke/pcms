using Core.Exceptions;
using Core.Repository;
using Core.Repository.Data;
using Core.Models;
using Domain.Utilities;
using Core.Cache;

namespace Domain.Services;

public class ProductService(
    IRepository<Core.Entities.Product, int> repository,
    IReadRepository<Core.Entities.Category, int> categoryRepository,
    ICacheProvider searchCache) : IProductService
{
    public async Task<IEnumerable<Product>> GetPagedProductsAsync(string? search, int? categoryId, int page, int pageSize)
    {
        string cacheKey = $"cat_{categoryId ?? 0}_search_{search?.ToLower().Trim() ?? "none"}";
        var allProducts = await Task.FromResult(searchCache.GetOrAdd(cacheKey, () =>
        {
            var entities = categoryId.HasValue
                ? repository.FindAsync(p => p.CategoryId == categoryId).GetAwaiter().GetResult()
                : repository.GetAllAsync().GetAwaiter().GetResult();

            var products = entities.Select(Core.Entities.Product.ToDomain).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var weights = new Dictionary<string, double> { { nameof(Product.Name), 1.0 }, { nameof(Product.Description), 0.5 } };
                var engine = new ProductSearchEngine<Product>(products, weights);
                products = engine.Search(search).ToList();
            }

            return products;
        }));

        return allProducts
                .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        return Core.Entities.Product.ToDomain(entity);
    }
    public async Task<Product> CreateProductAsync(Product domainProduct)
    {
        _ = await categoryRepository.GetByIdAsync(domainProduct.CategoryId)
            ?? throw new NotFoundException("Category", domainProduct.CategoryId);

        var entity = new Core.Entities.Product
        {
            Name = domainProduct.Name,
            SKU = domainProduct.SKU,
            Price = domainProduct.Price,
            Quantity = domainProduct.Quantity,
            CategoryId = domainProduct.CategoryId,
            Description = domainProduct.Description,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();
        //Invalidate the cache
        searchCache.Clear();
        return Core.Entities.Product.ToDomain(entity);
    }

    public async Task UpdateProductAsync(int id, Product domainProduct)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        entity.Name = domainProduct.Name;
        entity.Price = domainProduct.Price;
        entity.Quantity = domainProduct.Quantity;
        entity.Description = domainProduct.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(entity);
        await repository.SaveChangesAsync();
        //Invalidate the cache
        searchCache.Clear();
    }

    public async Task DeleteProductAsync(int id)
    {
        await repository.RemoveAsync(id);
        await repository.SaveChangesAsync();
        //Invalidate the cache
        searchCache.Clear();
    }
}