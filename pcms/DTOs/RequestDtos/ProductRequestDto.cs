namespace API.DTOs.RequestDtos
{
    public record CreateProductRequest(
        string Name,
        string SKU,
        decimal Price,
        int Quantity,
        int CategoryId,
        string? Description = null
    );
}
