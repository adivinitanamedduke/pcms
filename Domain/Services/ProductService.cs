using Core.Exceptions;
using Core.Repository;
using Core.Repository.Data;
using Data.Models;
using Domain.Utilities;

namespace Domain.Services;

public class ProductService(
    IRepository<Data.Entities.Product, int> repository,
    IReadRepository<Data.Entities.Category, int> categoryRepository) : IProductService
{
    public async Task<IEnumerable<Product>> GetPagedProductsAsync(string? search, int? categoryId, int page, int pageSize)
    {
        var entities = categoryId.HasValue
            ? await repository.FindAsync(p => p.CategoryId == categoryId)
            : await repository.GetAllAsync();

        var products = entities.Select(Data.Entities.Product.ToDomain).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var weights = new Dictionary<string, double> { { nameof(Product.Name), 1.0 }, { nameof(Product.Description), 0.5 } };
            var engine = new ProductSearchEngine<Product>(products, weights);
            products = engine.Search(search).ToList();
        }

        return products
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        return Data.Entities.Product.ToDomain(entity);
    }
    public async Task<Product> CreateProductAsync(Product domainProduct)
    {
        // Business Rule: Ensure category exists before creating product
        _ = await categoryRepository.GetByIdAsync(domainProduct.CategoryId)
            ?? throw new NotFoundException("Category", domainProduct.CategoryId);

        var entity = new Data.Entities.Product
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

        return Data.Entities.Product.ToDomain(entity);
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
    }

    public async Task DeleteProductAsync(int id)
    {
        await repository.RemoveAsync(id);
        await repository.SaveChangesAsync();
    }
}