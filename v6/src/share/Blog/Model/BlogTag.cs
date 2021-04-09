using System.Text.Json.Serialization;
using Laobian.Share.Util;

namespace Laobian.Share.Blog.Model
{
    public class BlogTag
    {
        public BlogTag(string link)
        {
            Link = link;
        }

        [JsonPropertyName("n")] public string Name { get; set; }

        [JsonPropertyName("l")] public string Link { get; }

        [JsonPropertyName("d")] public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is BlogTag t))
            {
                return false;
            }

            return StringUtil.EqualsIgnoreCase(Link, t.Link);
        }

        public override int GetHashCode()
        {
            return Link.GetHashCode() * 13;
        }
    }
}