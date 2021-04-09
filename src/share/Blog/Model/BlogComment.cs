using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Laobian.Share.Blog.Model
{
    public class BlogComment
    {
        [JsonPropertyName("l")] public string PostLink { get; set; }

        [JsonPropertyName("c")] public List<BlogCommentItem> CommentItems { get; set; }
    }
}