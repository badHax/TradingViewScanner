using System.Text.Json;

namespace TVScanner.Shared
{
    public class ObjectExtensions
    {
        public static string ToJson(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
