﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Blog.Repository;
using Laobian.Share.Extension;
using Laobian.Share.HttpService;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Blog.Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogReadonlyRepository _blogReadonlyRepository;
        private readonly IBlogReadWriteRepository _blogReadWriteRepository;
        private readonly ILogger<BlogService> _logger;
        private readonly ManualResetEventSlim _manualResetEventSlim;
        private readonly SemaphoreSlim _semaphoreSlim;

        private List<BlogPost> _allPosts;
        private List<BlogTag> _allTags;

        public BlogService(ILogger<BlogService> logger, IBlogReadonlyRepository blogReadonlyRepository,
            IBlogReadWriteRepository blogReadWriteRepository)
        {
            _logger = logger;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _manualResetEventSlim = new ManualResetEventSlim(true);
            _blogReadonlyRepository = blogReadonlyRepository;
            _blogReadWriteRepository = blogReadWriteRepository;
        }

        public List<BlogPost> GetAllPosts()
        {
            _manualResetEventSlim.Wait();
            return _allPosts;
        }

        public async Task PullGitFilesAsync()
        {
            var tasks = new List<Task> { _blogReadWriteRepository.PullAsync(), _blogReadonlyRepository.PullAsync() };
            await Task.WhenAll(tasks);
            _logger.LogInformation("Pull git files completed.");
        }

        public async Task PushGitFilesAsync(string commitMessage)
        {
            await _blogReadWriteRepository.PushAsync(commitMessage);
        }

        public async Task FlushDataToFileAsync()
        {
            await _blogReadWriteRepository.UpdatePostMetadataAsync(_allPosts.Select(x => x.Metadata).ToList());
            await _blogReadWriteRepository.UpdateAccessAsync(_allPosts.Select(x => x.Access).ToList());
            await _blogReadWriteRepository.UpdateCommentsAsync(_allPosts.Select(x => x.Comment).ToList());
            await _blogReadWriteRepository.UpdateTagsAsync(_allTags);
        }

        public async Task<bool> InitDataFromFileAsync()
        {
            _manualResetEventSlim.Reset();
            try
            {
                await PullGitFilesAsync();
                var allComments = await _blogReadWriteRepository.GetAllCommentsAsync();
                var allAccess = await _blogReadWriteRepository.GetAllAccessAsync();
                _allTags = await _blogReadWriteRepository.GetAllTagsAsync();
                var allPostMetadata = await _blogReadWriteRepository.GetAllPostMetadataAsync();
                _allPosts = await _blogReadonlyRepository.GetAllPostsAsync();

                foreach (var post in _allPosts)
                {
                    var metadata = allPostMetadata.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, post.Link));
                    if (metadata != null)
                    {
                        post.Metadata = metadata;
                    }
                    else
                    {
                        post.Metadata = new BlogPostMetadata
                        {
                            Link = post.Link,
                            Title = Guid.NewGuid().ToString(),
                            IsPublished = true,
                            PublishTime = DateTime.Now
                        };
                    }

                    var access = allAccess.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.PostLink, post.Link));
                    if (access != null)
                    {
                        post.Access = access;
                    }
                    else
                    {
                        post.Access = new BlogAccess {PostLink = post.Link};
                    }

                    var comment = allComments.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.PostLink, post.Link));
                    if (comment != null)
                    {
                        post.Comment = comment;
                        if (post.Comment.CommentItems == null)
                        {
                            post.Comment.CommentItems = new List<BlogCommentItem>();
                        }
                    }
                    else
                    {
                        post.Comment = new BlogComment {PostLink = post.Link};
                    }

                    post.Tags = _allTags.Where(x => post.Metadata.Tags.ContainsIgnoreCase(x.Link)).ToList();
                }

                _logger.LogInformation("Init data from git files finished");
                return true;
            }
            finally
            {
                _manualResetEventSlim.Set();
            }
        }

        public async Task<bool> UpdatePostMetadataAsync(BlogPostMetadata metadata)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var post = _allPosts.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, metadata.Link));
                    if (post == null)
                    {
                        _logger.LogError($"Failed to find post with link {metadata.Link}");
                        return false;
                    }

                    var existingMetadata = post.Metadata;
                    existingMetadata.Excerpt = metadata.Excerpt;
                    existingMetadata.IsPublished = metadata.IsPublished;
                    existingMetadata.PublishTime = metadata.PublishTime;
                    existingMetadata.Title = metadata.Title;
                    existingMetadata.AllowComment = metadata.AllowComment;
                    existingMetadata.ContainsMath = metadata.ContainsMath;
                    existingMetadata.IsTopping = metadata.IsTopping;
                    existingMetadata.LastUpdateTime = DateTime.Now;
                    existingMetadata.Tags = metadata.Tags;

                    post.Tags = _allTags.Where(x => post.Metadata.Tags.ContainsIgnoreCase(x.Link)).ToList();
                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> AddBlogTagAsync(BlogTag tag)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link));
                    if (existingTag != null)
                    {
                        _logger.LogError($"The target tag already exist. Tag link = {tag.Link}");
                        return false;
                    }

                    existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Name, tag.Name));
                    if (existingTag != null)
                    {
                        _logger.LogError($"The target tag already exist. Tag name = {tag.Name}");
                        return false;
                    }

                    _allTags.Add(tag);
                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> UpdateBlogTagAsync(BlogTag tag)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link));
                    if (existingTag == null)
                    {
                        _logger.LogError($"The target tag does not exist. Tag link = {tag.Link}");
                        return false;
                    }

                    existingTag.Name = tag.Name;
                    existingTag.Description = tag.Description;

                    foreach (var blogTag in _allPosts)
                    {
                        blogTag.Metadata.Tags = blogTag.Tags.Select(x => x.Link).ToList();
                    }

                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> RemoveBlogTagAsync(string link)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, link));
                    if (existingTag == null)
                    {
                        _logger.LogError($"The target tag does not exist. Tag link = {link}");
                        return false;
                    }

                    _allTags.Remove(existingTag);
                    foreach (var blogPost in _allPosts)
                    {
                        blogPost.Tags.Remove(existingTag);
                        blogPost.Metadata.Tags.Remove(link);
                    }

                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public List<BlogTag> GetAllBlogTags()
        {
            _manualResetEventSlim.Wait();
            return _allTags;
        }

        public async Task<bool> AddCommentAsync(string postLink, BlogCommentItem item)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var post = _allPosts.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, postLink));
                    if (post == null)
                    {
                        return false;
                    }

                    if (post.Comment == null)
                    {
                        post.Comment = new BlogComment();
                    }

                    if (post.Comment.CommentItems == null)
                    {
                        post.Comment.CommentItems = new List<BlogCommentItem>();
                    }

                    item.Id = Guid.NewGuid();
                    item.TimeStamp = DateTime.Now;
                    item.LastUpdatedAt = DateTime.Now;

                    // TODO: verify
                    post.Comment.CommentItems.Add(item);
                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<List<BlogComment>> GetCommentsAsync()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    return _allPosts.Select(x => x.Comment).ToList();
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<BlogComment> GetCommentAsync(string postLink)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var post = _allPosts.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, postLink));
                    return post?.Comment;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<BlogCommentItem> GetCommentItemAsync(Guid id)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var post = _allPosts.SelectMany(x => x.Comment.CommentItems);
                    return post.FirstOrDefault(x => x.Id == id);
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> UpdateCommentAsync(BlogCommentItem comment)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                _manualResetEventSlim.Reset();
                try
                {
                    var item = _allPosts.SelectMany(x => x.Comment.CommentItems)
                        .FirstOrDefault(x => comment.Id == x.Id);
                    if (item == null)
                    {
                        return false;
                    }

                    item.Email = comment.Email;
                    item.Ip = comment.Ip;
                    item.IsAdmin = comment.IsAdmin;
                    item.IsPublished = comment.IsPublished;
                    item.IsReviewed = comment.IsReviewed;
                    item.MarkdownContent = comment.MarkdownContent;
                    item.LastUpdatedAt = DateTime.Now;
                    item.UserName = comment.UserName;
                    return true;
                }
                finally
                {
                    _manualResetEventSlim.Set();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public List<BlogPostMetadata> GetAllPostMetadata()
        {
            _manualResetEventSlim.Wait();
            return _allPosts.Select(x => x.Metadata).ToList();
        }

        public List<BlogAccess> GetAllAccess()
        {
            _manualResetEventSlim.Wait();
            return _allPosts.Select(x => x.Access).ToList();
        }

        public List<BlogComment> GetAllComments()
        {
            _manualResetEventSlim.Wait();
            return _allPosts.Select(x => x.Comment).ToList();
        }

        //public async Task<bool> AddBlogCommentItemAsync(BlogCommentItem commentItem)
        //{
        //    try
        //    {
        //        await _semaphoreSlim.WaitAsync();
        //        _manualResetEventSlim.Reset();
        //        try
        //        {
        //            var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, commentItem.Link));
        //            if (existingTag != null)
        //            {
        //                _logger.LogError($"The target tag already exist. Tag link = {commentItem.Link}");
        //                return false;
        //            }

        //            existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Name, commentItem.Name));
        //            if (existingTag != null)
        //            {
        //                _logger.LogError($"The target tag already exist. Tag name = {commentItem.Name}");
        //                return false;
        //            }

        //            _allTags.Add(existingTag);
        //            return true;
        //        }
        //        finally
        //        {
        //            _manualResetEventSlim.Set();
        //        }
        //    }
        //    finally
        //    {
        //        _semaphoreSlim.Release();
        //    }
        //}

        //public async Task<bool> UpdateBlogTagAsync(BlogTag tag)
        //{
        //    try
        //    {
        //        await _semaphoreSlim.WaitAsync();
        //        _manualResetEventSlim.Reset();
        //        try
        //        {
        //            var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link));
        //            if (existingTag == null)
        //            {
        //                _logger.LogError($"The target tag does not exist. Tag link = {tag.Link}");
        //                return false;
        //            }

        //            if (_allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Name, tag.Name)) != null)
        //            {
        //                _logger.LogError($"The tag name already exist. Tag Name = {tag.Name}");
        //                return false;
        //            }

        //            existingTag = tag;
        //            return true;
        //        }
        //        finally
        //        {
        //            _manualResetEventSlim.Set();
        //        }
        //    }
        //    finally
        //    {
        //        _semaphoreSlim.Release();
        //    }
        //}

        //public async Task<bool> RemoveBlogTagAsync(BlogTag tag)
        //{
        //    try
        //    {
        //        await _semaphoreSlim.WaitAsync();
        //        _manualResetEventSlim.Reset();
        //        try
        //        {
        //            var existingTag = _allTags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link));
        //            if (existingTag == null)
        //            {
        //                _logger.LogError($"The target tag does not exist. Tag link = {tag.Link}");
        //                return false;
        //            }

        //            _allTags.Remove(existingTag);
        //            return true;
        //        }
        //        finally
        //        {
        //            _manualResetEventSlim.Set();
        //        }
        //    }
        //    finally
        //    {
        //        _semaphoreSlim.Release();
        //    }
        //}
    }
}