using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Laobian.Share.Converter;

namespace Laobian.Share.Util
{
    public static class JsonUtil
    {
        public static string Serialize<T>(T obj, bool writeIndented = false)
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = writeIndented
            };
            option.Converters.Add(new IsoDateTimeConverter());
            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}