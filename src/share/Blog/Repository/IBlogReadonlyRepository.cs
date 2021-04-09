using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Repository
{
    public interface IBlogReadonlyRepository
    {
        Task<List<BlogPost>> GetAllPostsAsync();

        Task PullAsync();
    }
}