using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Laobian.Share.Util;

namespace Laobian.Share.Blog.Model
{
    public class BlogPost
    {
        [JsonPropertyName("l")] public string Link { get; set; }

        [JsonPropertyName("m")] public BlogPostMetadata Metadata { get; set; }

        [JsonPropertyName("a")] public BlogAccess Access { get; set; }

        [JsonPropertyName("c")] public BlogComment Comment { get; set; }

        [JsonPropertyName("t")] public List<BlogTag> Tags { get; set; }

        [JsonPropertyName("md")] public string MarkdownContent { get; set; }

        private HtmlDocument GetHtmlDoc()
        {
            var html = MarkdownUtil.ToHtml(MarkdownContent);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        public string GetHtmlContent()
        {
            var htmlDoc = GetHtmlDoc();
            // TODO: Process Image nodes

            return htmlDoc.DocumentNode.OuterHtml;
        }

        public string GetExcerpt()
        {
            if (!string.IsNullOrEmpty(Metadata.Excerpt))
            {
                return $"<p>{Metadata.Excerpt}</p>";
            }

            var htmlDoc = GetHtmlDoc();
            var pNodes = htmlDoc.DocumentNode
                .Descendants()
                .Where(_ =>
                    StringUtil.EqualsIgnoreCase(_.Name, "p") &&
                    _.InnerText.Length > 10 &&
                    _.Descendants().FirstOrDefault(c => StringUtil.EqualsIgnoreCase(c.Name, "img")) == null).Take(2)
                .ToList();
            var excerpt = string.Empty;
            foreach (var node in pNodes)
            {
                excerpt += $"<p>{node.InnerText}</p>";
            }

            if (string.IsNullOrEmpty(excerpt))
            {
                return "<p><em>No except content available!</em></p>";
            }

            return excerpt;
        }

        public string GetFullLink()
        {
            return $"/{Metadata.PublishTime.ToString("yyyy")}/{Metadata.PublishTime.ToString("MM")}/{Link}.html";
        }

        public string GetPublishTimeString()
        {
            return Metadata.PublishTime.ToString("yyyy-MM-dd");
        }

        public bool IsPostPublished()
        {
            return Metadata.IsPublished && Metadata.PublishTime <= DateTime.Now;
        }

        public string GetAccessCountString()
        {
            return Access?.Access?.Sum(x => x.Value).ToString() ?? "0";
        }

        public int GetCommentCount()
        {
            return Comment?.CommentItems?.Count ?? 0;
        }
    }
}