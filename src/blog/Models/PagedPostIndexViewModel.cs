using System;
using System.Collections.Generic;

namespace Laobian.Blog.Models
{
    public class PagedPostIndexViewModel
    {
        public PagedPostIndexViewModel(int currentPage, int postCount, int postsPerPage)
        {
            if (postCount < 0) postCount = 0;

            TotalPages = Convert.ToInt32(Math.Ceiling(postCount / (double) postsPerPage));

            if (currentPage <= 0) currentPage = 1;

            if (currentPage > TotalPages) currentPage = TotalPages;

            CurrentPage = currentPage;
            Posts = new List<PostIndexViewModel>();
        }

        public int TotalPages { get; }

        public int CurrentPage { get; }

        public List<PostIndexViewModel> Posts { get; }

        public string Url { get; set; }
    }
}