using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500 Internal Server Error
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred.";

        // Custom logic for specific exception types
        if (exception is KeyNotFoundException) 
        {
            statusCode = HttpStatusCode.NotFound;
        } 
        else if (exception is UnauthorizedAccessException) 
        {
            statusCode = HttpStatusCode.Unauthorized;
        }
        else if (exception is ArgumentException) 
        { 
            statusCode = HttpStatusCode.BadRequest; 
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            context.Response.StatusCode,
            Message = message,
            // Only show stack trace in Development environment
            Detailed = env.IsDevelopment() ? exception.StackTrace : null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
