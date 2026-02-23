using Core.Exceptions;
using Core.Repository;
using Core.Models;
using Domain.Services.Interfaces;

namespace Domain.Services;

public class CategoryService(IRepository<Core.Entities.Category, int> categoryRepository) : ICategoryService
{
    private readonly IRepository<Core.Entities.Category, int> _categoryRepository = categoryRepository;

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);

        return Core.Entities.Category.ToDomain(entity);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return (await _categoryRepository.GetAllAsync()).Select(Core.Entities.Category.ToDomain);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        // Using the FindAsync from your repository to filter ParentCategoryId
        return (await _categoryRepository.FindAsync(c => c.ParentCategoryId == null)).Select(Core.Entities.Category.ToDomain);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        var entity = new Core.Entities.Category
        {
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId
        };
        await _categoryRepository.AddAsync(entity);
        await _categoryRepository.SaveChangesAsync();

        return Core.Entities.Category.ToDomain(entity);
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        var entity = new Core.Entities.Category
        {
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId
        };
        await _categoryRepository.UpdateAsync(entity);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _categoryRepository.RemoveAsync(id);
        await _categoryRepository.SaveChangesAsync();
    }
}

