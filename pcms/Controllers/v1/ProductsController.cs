using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.DTOs.RequestDtos;
using Domain.Services;
using Core.Models;

namespace API.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var items = await _productService.GetPagedProductsAsync(search, categoryId, page, pageSize);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product =  await _productService.GetProductByIdAsync(id);
        return product == null ? NotFound() : Ok(new ProductDto(product.Id, product.Name, product.Description, product.SKU, product.Price, product.Quantity, product.CategoryId, product.CreatedAt, product.UpdatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest req)
    {
        var product = new Product { Id = 0, Name = req.Name, SKU = req.SKU, Price = req.Price, Quantity = req.Quantity, CategoryId = req.CategoryId, Description = req.Description };
        product = await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductRequest req)
    {
        var product = new Product { Id = id, Name = req.Name, SKU = req.SKU, Price = req.Price, Quantity = req.Quantity, CategoryId = req.CategoryId, Description = req.Description };
        await _productService.UpdateProductAsync(id, product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
