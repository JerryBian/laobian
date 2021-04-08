using System.Text.Json.Serialization;

namespace Laobian.Share.Blog.Model
{
    public class BlogTag
    {
        [JsonPropertyName("n")] public string Name { get; set; }

        [JsonPropertyName("l")] public string Link { get; set; }

        [JsonPropertyName("d")] public string Description { get; set; }
    }
}