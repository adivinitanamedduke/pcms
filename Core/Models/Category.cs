namespace Core.Models;
public class Category
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public List<Category> SubCategories { get; init; } = [];
    public List<Product> Products { get; init; } = [];
    public bool IsRoot => !ParentCategoryId.HasValue;
}

