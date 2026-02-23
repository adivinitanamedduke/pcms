using API.DTOs.RequestDtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace API.Middleware;

public class ProductValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate Product creation or updates
        if ((context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            && context.Request.Path.Value?.Contains("/api/products") == true)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var request = JsonSerializer.Deserialize<CreateProductRequest>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Pattern Matching Logic
            var validationError = request switch
            {
                null => "Request body cannot be empty.",
                { Name: null or { Length: 0 } } => "Product name is required.",
                { Price: <= 0 } => "Price must be a positive value.",
                { Quantity: < 0 } => "Stock quantity cannot be negative.",
                { SKU.Length: < 5 or > 50 } => "SKU must be between 5 and 50 characters.",
                _ => null
            };

            if (validationError is not null)
            {
                throw new ValidationException(validationError);
            }
        }

        await next(context);
    }
}

