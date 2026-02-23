namespace API.DTOs
{
    public record CategoryDto(
        int Id,
        string Name,
        string? Description,
        int? ParentCategoryId,
        IEnumerable<CategoryDto>? SubCategories = null
    );
}
