using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class PostIndexViewModel
    {
        public string Link { get; set; }

        public string Access { get; set; }

        public string Excerpt { get; set; }

        public string PublishTime { get; set; }

        public string Title { get; set; }

        public List<BlogTag> Tags { get; set; }

        public int Comments { get; set; }
    }
}