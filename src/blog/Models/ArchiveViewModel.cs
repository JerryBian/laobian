using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class ArchiveViewModel
    {
        public string DateKey { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}