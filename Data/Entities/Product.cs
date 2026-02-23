using Data.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Product : IComparable<Product>, IMappable<Models.Product>
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public static Models.Product ToDomain(object entity)
    {
        var e = (Product)entity;
        return new Models.Product
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            SKU = e.SKU,
            Price = e.Price,
            Quantity = e.Quantity,
            CategoryId = e.CategoryId,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };
    }

    /// <summary>
    /// Default sorting: Name (Alphabetical), then by Price (Low to High)
    /// </summary>
    public int CompareTo(Product? product)
    {
        if (product is null) return 1;

        int nameComparison = string.Compare(this.Name, product.Name, StringComparison.OrdinalIgnoreCase);
        if (nameComparison != 0) return nameComparison;

        return this.Price.CompareTo(product.Price);
    }
}

