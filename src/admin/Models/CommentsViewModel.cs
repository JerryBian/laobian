using System.Text.Json.Serialization;
using Laobian.Share.Blog.Model;

namespace Laobian.Admin.Models
{
    public class CommentsViewModel
    {
        [JsonPropertyName("p")] public BlogPost Post { get; set; }

        [JsonPropertyName("c")] public BlogCommentItem Comment { get; set; }
    }
}