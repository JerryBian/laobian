using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Admin.Models
{
    public class PostsViewModel
    {
        public List<BlogPost> Posts { get; set; }

        public List<BlogTag> Tags { get; set; }
    }
}