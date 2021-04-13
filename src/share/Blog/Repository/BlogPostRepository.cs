using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Command;
using Laobian.Share.Setting;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Blog.Repository
{
    public class BlogPostRepository : IBlogReadonlyRepository
    {
        private readonly BlogSetting _blogSetting;
        private readonly ICommandClient _commandClient;
        private readonly ILogger<BlogPostRepository> _logger;

        public BlogPostRepository(IOptions<BlogSetting> setting, ICommandClient commandClient,
            ILogger<BlogPostRepository> logger)
        {
            _logger = logger;
            _commandClient = commandClient;
            _blogSetting = setting.Value;
        }

        public async Task PullAsync()
        {
            if (string.IsNullOrEmpty(_blogSetting.GitHubReadonlyRepoLocalDir))
            {
                _logger.LogWarning("Pull posts failed, local dir not setup.");
                return;
            }

            if (Directory.Exists(_blogSetting.GitHubReadonlyRepoLocalDir))
            {
                DirectoryUtil.Delete(_blogSetting.GitHubReadonlyRepoLocalDir);
            }

            var repoUrl =
                $"https://{_blogSetting.GitHubReadonlyRepoApiToken}@github.com/{_blogSetting.GitHubReadonlyRepoOwner}/{_blogSetting.GitHubReadonlyRepoName}.git";
            var command =
                $"{_blogSetting.CommandLineArgQuote}git clone -b {_blogSetting.GitHubReadonlyRepoBranch} --single-branch {repoUrl} {_blogSetting.GitHubReadonlyRepoLocalDir}{_blogSetting.CommandLineArgQuote}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }


        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            var posts = new List<BlogPost>();
            if (string.IsNullOrEmpty(_blogSetting.GitHubReadonlyRepoLocalDir) ||
                !Directory.Exists(_blogSetting.GitHubReadonlyRepoLocalDir)) return posts;

            var links = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in Directory.EnumerateFiles(_blogSetting.GitHubReadonlyRepoLocalDir, "*.md",
                SearchOption.AllDirectories))
            {
                var link = Path.GetFileNameWithoutExtension(item);
                if (links.Contains(link))
                {
                    // duplicate link is not allowed
                    _logger.LogWarning($"Duplicate post link detected, ignore => {link}");
                    continue;
                }

                links.Add(link);
                var post = new BlogPost {Link = link};
                post.MarkdownContent = await File.ReadAllTextAsync(item);
                posts.Add(post);
            }

            return posts;
        }
    }
}