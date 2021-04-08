using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog.Model
{
    public class BlogPostMetadata
    {
        [JsonPropertyName("l")] public string Link { get; set; }

        [JsonPropertyName("t")] public string Title { get; set; }

        [JsonPropertyName("c")] public DateTime CreateTime { get; set; } = DateTime.Now;

        [JsonPropertyName("p")] public DateTime PublishTime { get; set; } = DateTime.Now;

        [JsonPropertyName("lu")] public DateTime LastUpdateTime { get; set; } = DateTime.Now;

        [JsonPropertyName("ip")] public bool IsPublished { get; set; }

        [JsonPropertyName("it")] public bool IsTopping { get; set; }

        [JsonPropertyName("cm")] public bool ContainsMath { get; set; }

        [JsonPropertyName("a")] public bool AllowComment { get; set; }

        [JsonPropertyName("e")] public string Excerpt { get; set; }

        [JsonPropertyName("ta")] public List<string> Tags { get; set; } = new List<string>();
    }
}