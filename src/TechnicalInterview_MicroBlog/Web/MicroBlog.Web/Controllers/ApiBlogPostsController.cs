﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.Db;
using Microsoft.AspNet.Identity;

namespace MicroBlog.Web.Controllers
{
    public class ApiBlogPostsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Wrap linq expression in Func, compile, and invoke. This allows me to define the mapping in one place.
        // A bit odd...but at least it's DRY.
        private static readonly Func<BlogPost, BlogPostApiDto> AsBlogPostApiDto = blogPost => AsBlogPostApiDtoExpression.Compile().Invoke(blogPost);

        // Convert BlogPost to BlogPostApiDto
        private static readonly Expression<Func<BlogPost, BlogPostApiDto>> AsBlogPostApiDtoExpression =
            blogPost => new BlogPostApiDto
            {
                BlogPostId = blogPost.BlogPostId,
                Title = blogPost.Title,
                Content = blogPost.Content,
                ApplicationUserId = blogPost.ApplicationUserId
            };

        // Convert BlogPostApiDto to BlogPost
        private static readonly Func<BlogPostApiDto, BlogPost> AsBlogPost = blogPostApiDto => new BlogPost
        {
            BlogPostId = blogPostApiDto.BlogPostId,
            Title = blogPostApiDto.Title,
            Content = blogPostApiDto.Content,
            ApplicationUserId = blogPostApiDto.ApplicationUserId
        };

        // GET: api/BlogPosts
        [Route("api/BlogPosts")]
        public IQueryable<BlogPostApiDto> GetBlogPosts()
        {
            return db.BlogPosts.Select(AsBlogPostApiDtoExpression);
        }

        // GET: api/BlogPosts/5
        [Route("api/BlogPosts/{id:int}", Name = "GetBlogPost")]
        [ResponseType(typeof(BlogPostApiDto))]
        public async Task<IHttpActionResult> GetBlogPost(int id)
        {
            BlogPostApiDto blogPostApiDto = await db.BlogPosts
                .Where(b => b.BlogPostId == id)
                .Select(AsBlogPostApiDtoExpression)
                .FirstOrDefaultAsync();

            if (blogPostApiDto == null)
            {
                return NotFound();
            }

            return Ok(blogPostApiDto);
        }

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/BlogPosts
        [Route("api/ApplicationUsers/{ApplicationUserId}/BlogPosts", Name = "GetBlogPostsByApplicationUser")]
        public IQueryable<BlogPostApiDto> GetBlogPostsByApplicationUser(string applicationUserId)
        {
            return db.BlogPosts.Include(b => b.ApplicationUser)
                .Where(b => b.ApplicationUserId.Equals(applicationUserId))
                .Select(AsBlogPostApiDtoExpression);
        }

        // PUT: api/BlogPosts/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutBlogPost(int id, BlogPost blogPost)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != blogPost.BlogPostId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(blogPost).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BlogPostExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //private bool BlogPostExists(int id)
        //{
        //    return db.BlogPosts.Count(e => e.BlogPostId == id) > 0;
        //}

        // POST: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/BlogPosts
        [Route("api/ApplicationUsers/{ApplicationUserId}/BlogPosts", Name = "PostBlogPostsByApplicationUser")]
        [ResponseType(typeof(BlogPostApiDto))]
        public async Task<IHttpActionResult> PostBlogPostByApplicationUser(string applicationUserId, BlogPostApiDto blogPostApiDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var blogPost = AsBlogPost(blogPostApiDto);
            blogPost.ApplicationUserId = applicationUserId; // Set ApplicationUserId to applicationUserId URI segment value
            db.BlogPosts.Add(blogPost);
            await db.SaveChangesAsync();

            return CreatedAtRoute("GetBlogPost", new { id = blogPost.BlogPostId }, AsBlogPostApiDto(blogPost));
        }

        // DELETE: api/BlogPosts/5
        //[ResponseType(typeof(BlogPost))]
        //public async Task<IHttpActionResult> DeleteBlogPost(int id)
        //{
        //    BlogPost blogPost = await db.BlogPosts.FindAsync(id);
        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    db.BlogPosts.Remove(blogPost);
        //    await db.SaveChangesAsync();

        //    return Ok(blogPost);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}