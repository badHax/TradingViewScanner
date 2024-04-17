using System.Text.Json;

namespace TVScanner.Shared
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
