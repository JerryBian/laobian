using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Service
{
    public interface IBlogService
    {
        Task FlushDataAsync();

        Task<bool> ReloadAsync();

        List<BlogPost> GetAllPosts();

        List<BlogTag> GetAllBlogTags();

        Task PullGitFilesAsync();

        Task PushGitFilesAsync(string commitMessage);

        Task<bool> UpdatePostMetadataAsync(BlogPostMetadata metadata);

        Task<bool> AddBlogTagAsync(BlogTag tag);

        Task<bool> UpdateBlogTagAsync(BlogTag tag);

        Task<bool> RemoveBlogTagAsync(string link);

        Task<bool> AddCommentAsync(string postLink, BlogCommentItem item);
    }
}