using Newtonsoft.Json;

namespace Shared.Extensions
{
    public static class JsonExtension
    {
        public static T TryDeserialize<T>(this string str)
        {
            return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject<T>(str);
        }

        public static T TryDeserialize<T>(this string str, JsonSerializerSettings settings)
        {
            return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject<T>(str, settings);
        }
    }
}