using System.Text.Json;

namespace Backbone.Core.Helpers
{
    public static class JsonHelper
    {
        public static string Serialize(object obj, bool indented = false)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = indented
            };
            return JsonSerializer.Serialize(obj, options);
        }

        public static T? Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }


}
