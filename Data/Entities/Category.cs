using Data.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Category : IMappable<Models.Category>
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public static Models.Category ToDomain(object entity)

    {
        var e = (Category)entity;

        return new Models.Category
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            ParentCategoryId = e.ParentCategoryId,
            // Recursive mapping
            SubCategories = e.SubCategories
                .Select(child => Category.ToDomain(child))
                .ToList(),
            Products = e.Products
                .Select(p => Product.ToDomain(p))
                .ToList()
        };
    }
}
