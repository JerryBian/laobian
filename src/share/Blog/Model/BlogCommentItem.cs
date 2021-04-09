using System;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog.Model
{
    public class BlogCommentItem
    {
        [JsonPropertyName("i")] public Guid Id { get; set; }

        [JsonPropertyName("t")] public DateTime TimeStamp { get; set; }

        [JsonPropertyName("u")] public string User { get; set; }

        [JsonPropertyName("e")] public string Email { get; set; }

        [JsonPropertyName("a")] public bool IsAdmin { get; set; }

        [JsonPropertyName("md")] public string MarkdownContent { get; set; }
    }
}