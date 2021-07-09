﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Service
{
    public class BlogService : IBlogService
    {
        private readonly IDbRepository _dbRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogService(IDbRepository dbRepository, IBlogPostRepository blogPostRepository)
        {
            _dbRepository = dbRepository;
            _blogPostRepository = blogPostRepository;
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(_blogPostRepository.LoadAsync(cancellationToken),
                _dbRepository.LoadAsync(cancellationToken));
            await AggregateStoreAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            return allPosts;
        }

        public async Task<List<BlogTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            var allBlogs = blogTagStore.GetAll();
            return allBlogs;
        }

        public async Task<BlogPost> GetPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            return blogPostStore.GetByLink(postLink);
        }

        public async Task<BlogTag> GetTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            return blogTagStore.GetByLink(tagLink);
        }

        public async Task AddBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.Add(tag);
        }

        public async Task UpdateBlogTagAsync(BlogTag tag, CancellationToken cancellationToken = default)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.Update(tag);
        }

        public async Task RemoveBlogTagAsync(string tagLink, CancellationToken cancellationToken = default)
        {
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var allPosts = blogPostStore.GetAll();
            allPosts.ForEach(x => x.Tags.RemoveAll(y => StringHelper.EqualIgnoreCase(y.Link, tagLink)));

            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            blogTagStore.RemoveByLink(tagLink);
        }

        public async Task UpdateBlogPostMetadataAsync(BlogPostMetadata metadata, CancellationToken cancellationToken = default)
        {
            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            blogMetadataStore.Update(metadata);
        }

        public async Task AddBlogAccessAsync(string postLink, CancellationToken cancellationToken = default)
        {
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            blogAccessStore.Add(postLink, DateTime.Now, 1);
        }

        private async Task AggregateStoreAsync(CancellationToken cancellationToken)
        {
            var blogTagStore = await _dbRepository.GetBlogTagStoreAsync(cancellationToken);
            var blogPostStore = await _blogPostRepository.GetBlogPostStoreAsync(cancellationToken);
            var blogAccessStore = await _dbRepository.GetBlogAccessStoreAsync(cancellationToken);
            var blogMetadataStore = await _dbRepository.GetBlogMetadataStoreAsync(cancellationToken);
            var blogCommentStore = await _dbRepository.GetBlogCommentStoreAsync(cancellationToken);

            foreach (var blogPost in blogPostStore.GetAll())
            {
                var metadata = blogMetadataStore.GetByLink(blogPost.Link);
                if (metadata == null)
                {
                    metadata = new BlogPostMetadata { Link = blogPost.Link};
                    blogMetadataStore.Add(metadata);
                }

                blogPost.Metadata = metadata;
                var access = blogAccessStore.GetByLink(blogPost.Link);
                blogPost.Accesses.AddRange(access);
                blogPost.TotalAccess = access.Sum(x => x.Count);

                foreach (var metadataTag in metadata.Tags)
                {
                    var tag = blogTagStore.GetByLink(metadataTag);
                    if (tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                }

                var comments = blogCommentStore.GetByLink(blogPost.Link);
                blogPost.Comments.AddRange(comments);
            }
        }
    }
}