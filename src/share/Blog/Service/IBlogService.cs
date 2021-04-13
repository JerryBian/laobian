using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Service
{
    public interface IBlogService
    {
        Task FlushDataToFileAsync();

        Task<bool> InitDataFromFileAsync();

        List<BlogPost> GetAllPosts();

        List<BlogTag> GetAllBlogTags();

        Task PullGitFilesAsync();

        Task PushGitFilesAsync(string commitMessage);

        Task<bool> UpdatePostMetadataAsync(BlogPostMetadata metadata);

        Task<bool> AddBlogTagAsync(BlogTag tag);

        Task<bool> UpdateBlogTagAsync(BlogTag tag);

        Task<bool> RemoveBlogTagAsync(string link);

        Task<bool> AddCommentAsync(string postLink, BlogCommentItem item);

        Task<bool> UpdateCommentAsync(BlogCommentItem comment);

        Task<BlogComment> GetCommentAsync(string postLink);

        Task<List<BlogComment>> GetCommentsAsync();

        Task<BlogCommentItem> GetCommentItemAsync(Guid id);
    }
}