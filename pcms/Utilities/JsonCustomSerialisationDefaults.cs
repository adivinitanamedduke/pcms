using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Utilities
{
    public static class JsonSerializationDefaults
    {
        public static JsonSerializerOptions TreeOptions => new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }
}
