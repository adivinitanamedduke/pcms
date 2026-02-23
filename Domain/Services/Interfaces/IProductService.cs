using Core.Models;

namespace Domain.Services;
public interface IProductService
{
    Task<IEnumerable<Product>> GetPagedProductsAsync(string? search, int? categoryId, int page, int pageSize);
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(int id, Product product);
    Task DeleteProductAsync(int id);
}