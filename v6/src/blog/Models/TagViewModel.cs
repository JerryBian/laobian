using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
