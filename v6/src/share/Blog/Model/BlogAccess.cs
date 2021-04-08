using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog.Model
{
    public class BlogAccess
    {
        [JsonPropertyName("l")] public string PostLink { get; set; }

        [JsonPropertyName("a")]
        public ConcurrentDictionary<string, int> Access { get; set; } = new ConcurrentDictionary<string, int>();
    }
}