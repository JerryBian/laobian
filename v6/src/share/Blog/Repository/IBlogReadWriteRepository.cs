using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Repository
{
    public interface IBlogReadWriteRepository
    {
        Task<List<BlogPostMetadata>> GetAllPostMetadataAsync();

        Task UpdatePostMetadataAsync(List<BlogPostMetadata> metadata);

        Task<List<BlogTag>> GetAllTagsAsync();

        Task UpdateTagsAsync(List<BlogTag> tags);

        Task<List<BlogAccess>> GetAllAccessAsync();

        Task UpdateAccessAsync(List<BlogAccess> access);

        Task<List<BlogComment>> GetAllCommentsAsync();

        Task UpdateCommentsAsync(List<BlogComment> comments);

        Task PullAsync();

        Task PushAsync(string commitMessage);
    }
}