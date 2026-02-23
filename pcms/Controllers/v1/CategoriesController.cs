using API.DTOs;
using API.Utilities;
using Data.Models;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers.v1;


[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories() =>
        (await _categoryService.GetAllCategoriesAsync())
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ParentCategoryId)).ToList();

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoryTree()
    {
        var all = await _categoryService.GetAllCategoriesAsync();

        // Recursive tree builder
        List<CategoryDto> BuildTree(int? parentId) =>
            all.Where(c => c.ParentCategoryId == parentId)
               .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ParentCategoryId, BuildTree(c.Id)))
               .ToList();

        return Ok(BuildTree(null));
    }

    //[HttpGet("tree")]
    //public async Task<IActionResult> GetCategoryTree()
    //{
    //    // 1. Fetch from our In-Memory Repository
    //    var allEntities = await _categoryService.GetAllCategoriesAsync();

    //    // 2. Build the Domain Tree (using our previously defined logic)
    //    var rootCategories = allEntities
    //        .Where(c => c.ParentCategoryId == null)
    //        .Select(Category.ToDomain)
    //        .ToList();

    //    // 3. Custom Manual Serialization
    //    string jsonResult = JsonSerializer.Serialize(rootCategories, JsonSerializationDefaults.TreeOptions);

    //    // 4. Return as application/json
    //    return Content(jsonResult, "application/json");
    //}


    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto req)
    {
        var category = new Category { Id = 0, Name = req.Name, Description = req.Description, ParentCategoryId = req.ParentCategoryId };
        category = await _categoryService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }
}

