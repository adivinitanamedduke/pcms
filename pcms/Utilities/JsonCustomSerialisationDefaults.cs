using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Utilities
{
    public static class JsonSerializationDefaults
    {
        public static JsonSerializerOptions TreeOptions => new()
        {
            // Handle potential circular references in the parent-child relationship
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // Don't send empty SubCategories or Descriptions to save bandwidth
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Pretty print for debugging purposes
            WriteIndented = true
        };
    }
}
