using System;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;

namespace Laobian.Share.Blog.Model
{
    public class BlogCommentItem
    {
        private HtmlDocument _htmlDoc;

        [JsonPropertyName("i")] public Guid Id { get; set; }

        [JsonPropertyName("t")] public DateTime TimeStamp { get; set; }

        [JsonPropertyName("u")] public string UserName { get; set; }

        [JsonPropertyName("e")] public string Email { get; set; }

        [JsonPropertyName("a")] public bool IsAdmin { get; set; }

        [JsonPropertyName("md")] public string MarkdownContent { get; set; }

        [JsonPropertyName("p")] public string Ip { get; set; }

        [JsonPropertyName("ir")] public bool IsReviewed { get; set; }

        [JsonPropertyName("ip")] public bool IsPublished { get; set; }

        [JsonPropertyName("l")] public DateTime LastUpdatedAt { get; set; }

        public string GetHtmlContent()
        {
            if (_htmlDoc == null)
            {
                _htmlDoc = new HtmlDocument();
                _htmlDoc.LoadHtml(MarkdownUtil.ToHtml(MarkdownContent));
            }

            return _htmlDoc.DocumentNode.OuterHtml;
        }

        public string GetId()
        {
            return Id.ToString("N");
        }
    }
}