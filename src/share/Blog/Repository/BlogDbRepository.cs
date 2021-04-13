using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Command;
using Laobian.Share.Setting;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.Blog.Repository
{
    public class BlogDbRepository : IBlogReadWriteRepository
    {
        private readonly ICommandClient _commandClient;
        private readonly ILogger<BlogDbRepository> _logger;
        private readonly BlogSetting _setting;

        public BlogDbRepository(IOptions<BlogSetting> setting, ICommandClient commandClient,
            ILogger<BlogDbRepository> logger)
        {
            _logger = logger;
            _commandClient = commandClient;
            _setting = setting.Value;
        }

        public async Task PullAsync()
        {
            if (string.IsNullOrEmpty(_setting.GitHubReadWriteRepoLocalDir))
            {
                _logger.LogWarning("Pull posts failed, local dir not setup.");
                return;
            }

            if (Directory.Exists(_setting.GitHubReadWriteRepoLocalDir))
            {
                DirectoryUtil.Delete(_setting.GitHubReadWriteRepoLocalDir);
            }

            var repoUrl =
                $"https://{_setting.GitHubReadWriteRepoApiToken}@github.com/{_setting.GitHubReadWriteRepoOwner}/{_setting.GitHubReadWriteRepoName}.git";
            var commands = new List<string>();
            commands.Add(
                $"git clone -b {_setting.GitHubReadWriteRepoBranch} --single-branch {repoUrl} {_setting.GitHubReadWriteRepoLocalDir}");
            commands.Add($"cd {_setting.GitHubReadWriteRepoLocalDir}");
            commands.Add($"git config --local user.name \"{_setting.GitHubReadWriteRepoCommitUser}\"");
            commands.Add($"git config --local user.email \"{_setting.GitHubReadWriteRepoCommitEmail}\"");
            var command =
                $"{_setting.CommandLineArgQuote}{string.Join(" && ", commands)}{_setting.CommandLineArgQuote}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        public async Task PushAsync(string commitMessage)
        {
            if (string.IsNullOrEmpty(_setting.GitHubReadWriteRepoLocalDir))
            {
                _logger.LogWarning("Push posts failed, local dir not setup.");
                return;
            }

            if (!Directory.Exists(_setting.GitHubReadWriteRepoLocalDir))
            {
                _logger.LogWarning("Push post failed, local dir not exist.");
                return;
            }

            var commands = new List<string>();
            commands.Add($"cd {_setting.GitHubReadWriteRepoLocalDir}");
            commands.Add("git add .");
            commands.Add($"git commit -m \"{commitMessage}\"");
            commands.Add("git push");
            var command =
                $"{_setting.CommandLineArgQuote}{string.Join(" && ", commands)}{_setting.CommandLineArgQuote}";
            var output = await _commandClient.RunAsync(command);
            _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
        }

        public async Task<List<BlogPostMetadata>> GetAllPostMetadataAsync()
        {
            var metadata = new List<BlogPostMetadata>();
            if (string.IsNullOrEmpty(_setting.PostMetadataFileName)) return metadata;

            var metadataFile = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.PostMetadataFileName);
            if (!File.Exists(metadataFile)) return metadata;

            var content = await File.ReadAllTextAsync(metadataFile, Encoding.UTF8);
            return JsonUtil.Deserialize<List<BlogPostMetadata>>(content);
        }

        public async Task UpdatePostMetadataAsync(List<BlogPostMetadata> metadata)
        {
            if (string.IsNullOrEmpty(_setting.PostMetadataFileName))
            {
                _logger.LogError($"No metadata file specified in setting {nameof(_setting.PostMetadataFileName)}");
                return;
            }

            var metadataFile = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.PostMetadataFileName);
            var dir = Path.GetDirectoryName(metadataFile);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var content = JsonUtil.Serialize(metadata, true);
            await File.WriteAllTextAsync(metadataFile, content, Encoding.UTF8);
        }

        public async Task<List<BlogTag>> GetAllTagsAsync()
        {
            var tags = new List<BlogTag>();
            if (string.IsNullOrEmpty(_setting.TagFilePath)) return tags;

            var tagFile = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.TagFilePath);
            if (!File.Exists(tagFile)) return tags;

            var content = await File.ReadAllTextAsync(tagFile, Encoding.UTF8);
            return JsonUtil.Deserialize<List<BlogTag>>(content);
        }

        public async Task UpdateTagsAsync(List<BlogTag> tags)
        {
            if (string.IsNullOrEmpty(_setting.TagFilePath))
            {
                _logger.LogError($"No tag file specified in setting {nameof(_setting.TagFilePath)}");
                return;
            }

            var tagFile = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.TagFilePath);
            var dir = Path.GetDirectoryName(tagFile);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var content = JsonUtil.Serialize(tags, true);
            await File.WriteAllTextAsync(tagFile, content, Encoding.UTF8);
        }

        public async Task<List<BlogAccess>> GetAllAccessAsync()
        {
            var access = new List<BlogAccess>();
            if (string.IsNullOrEmpty(_setting.AccessDir)) return access;

            var accessDir = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.AccessDir);
            if (!Directory.Exists(accessDir)) return access;

            foreach (var item in Directory.EnumerateFiles(accessDir, "*.txt", SearchOption.AllDirectories))
            {
                var postAccess = new BlogAccess
                {
                    PostLink = Path.GetFileNameWithoutExtension(item),
                    Access = JsonUtil.Deserialize<ConcurrentDictionary<string, int>>(await File.ReadAllTextAsync(item))
                };
                access.Add(postAccess);
            }

            return access;
        }

        public async Task UpdateAccessAsync(List<BlogAccess> access)
        {
            if (access == null) return;

            if (string.IsNullOrEmpty(_setting.AccessDir))
            {
                _logger.LogError($"No access dir specified in setting {nameof(_setting.AccessDir)}");
                return;
            }

            var accessDir = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.AccessDir);
            Directory.CreateDirectory(accessDir);

            foreach (var item in access)
                await File.WriteAllTextAsync(Path.Combine(accessDir, $"{item.PostLink}.txt"),
                    JsonUtil.Serialize(item.Access), Encoding.UTF8);
        }

        public async Task<List<BlogComment>> GetAllCommentsAsync()
        {
            var comments = new List<BlogComment>();
            if (string.IsNullOrEmpty(_setting.CommentDir)) return comments;

            var commentDir = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.CommentDir);
            if (!Directory.Exists(commentDir)) return comments;

            foreach (var item in Directory.EnumerateFiles(commentDir, "*.json", SearchOption.AllDirectories))
            {
                var comment = new BlogComment {PostLink = Path.GetFileNameWithoutExtension(item)};
                comment.CommentItems =
                    JsonUtil.Deserialize<List<BlogCommentItem>>(await File.ReadAllTextAsync(item, Encoding.UTF8));
                comments.Add(comment);
            }

            return comments;
        }

        public async Task UpdateCommentsAsync(List<BlogComment> comments)
        {
            if (comments == null) return;

            if (string.IsNullOrEmpty(_setting.CommentDir))
            {
                _logger.LogError($"No comment dir specified in setting {nameof(_setting.CommentDir)}");
                return;
            }

            var commentDir = Path.Combine(_setting.GitHubReadWriteRepoLocalDir, _setting.CommentDir);
            Directory.CreateDirectory(commentDir);

            foreach (var item in comments)
                await File.WriteAllTextAsync(Path.Combine(commentDir, $"{item.PostLink}.json"),
                    JsonUtil.Serialize(item.CommentItems), Encoding.UTF8);
        }
    }
}