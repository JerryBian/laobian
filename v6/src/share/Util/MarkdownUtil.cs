using Markdig;

namespace Laobian.Share.Util
{
    public static class MarkdownUtil
    {
        public static string ToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown);
        }
    }
}