namespace Core.Models;

public class Product : IComparable<Product>
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string SKU { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsInStock => Quantity > 0;

    public int CompareTo(Product? other)
    {
        if (other is null)
        {
            return 1;
        }
        int nameComparison = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        return nameComparison != 0 ? nameComparison : Price.CompareTo(other.Price);
    }
}

