using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class TagViewModel
    {
        public string Key { get; set; }

        public BlogTag Tag { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}