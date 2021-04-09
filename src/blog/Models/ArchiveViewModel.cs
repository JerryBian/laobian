using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class ArchiveViewModel
    {
        public string DateKey { get; set; }

        public List<BlogPost> Posts { get; set; }
    }
}
