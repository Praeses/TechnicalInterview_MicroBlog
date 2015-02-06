using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.Db;
using MicroBlog.Web.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace MicroBlog.Web.Controllers
{
    public class MvcBlogPostsController : Controller
    {
        // Convert BlogPostApiDto to BlogPostViewModel
        private static readonly Func<BlogPostApiDto, BlogPostViewModel> AsBlogPostViewModel =
            blogPostApiDto => new BlogPostViewModel
            {
                BlogPostId = blogPostApiDto.BlogPostId,
                Title = blogPostApiDto.Title,
                Content = blogPostApiDto.Content,
                ApplicationUserId = blogPostApiDto.ApplicationUserId
            };

        // Convert BlogPostViewModel to BlogPostApiDto
        private static readonly Func<BlogPostViewModel, BlogPostApiDto> AsBlogPostApiDto =
            blogPostViewModel => new BlogPostApiDto
            {
                BlogPostId = blogPostViewModel.BlogPostId,
                Title = blogPostViewModel.Title,
                Content = blogPostViewModel.Content,
                ApplicationUserId = blogPostViewModel.ApplicationUserId
            };

        // GET: BlogPosts
        public async Task<ActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = UrlHelper.BuildRequestUri("GetBlogPostsByApplicationUser",
                    new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
                var httpResponseMessage = await client.GetAsync(requestUri);

                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var model = JsonConvert.DeserializeObject<IEnumerable<BlogPostApiDto>>(content);
                    return View(model.Select(AsBlogPostViewModel));
                }

                // an error occurred => here you could log the content returned by the remote server
                return Content("An error occurred: " + content);
            }
        }

        //// GET: MvcBlogPosts/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    using (HttpClient client = new HttpClient())
        //    {
        //        var requestUri = Url.RouteUrl("DefaultApi", new { httproute = "", controller = "ApiBlogPosts", id }, Request.Url.Scheme);
        //        var httpResponseMessage = await client.GetAsync(requestUri);

        //        string content = await httpResponseMessage.Content.ReadAsStringAsync();
        //        if (httpResponseMessage.IsSuccessStatusCode)
        //        {
        //            var model = JsonConvert.DeserializeObject<BlogPostApiDto>(content);
        //            return View(AsBlogPostViewModel(model));
        //        }

        //        // an error occurred => here you could log the content returned by the remote server
        //        return Content("An error occurred: " + content);
        //    }
        //}

        // GET: MvcBlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MvcBlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BlogPostId,Title,Content")] BlogPostViewModel blogPostViewModel)
        {
            if (ModelState.IsValid)
            {
                using (HttpClient client = new HttpClient())
                {
                    var requestUri = UrlHelper.BuildRequestUri("PostBlogPostsByApplicationUser",
                        new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
                    var httpResponseMessage = await client.PostAsJsonAsync(requestUri, AsBlogPostApiDto(blogPostViewModel));

                    string content = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    // an error occurred => here you could log the content returned by the remote server
                    return Content("An error occurred: " + content);
                }
            }

            return View(blogPostViewModel);
        }

        //// GET: MvcBlogPosts/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPost blogPost = await db.BlogPosts.FindAsync(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}

        //// POST: MvcBlogPosts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "BlogPostId,Title,Content")] BlogPost blogPost)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(blogPost).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(blogPost);
        //}

        //// GET: MvcBlogPosts/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPost blogPost = await db.BlogPosts.FindAsync(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}

        //// POST: MvcBlogPosts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    BlogPost blogPost = await db.BlogPosts.FindAsync(id);
        //    db.BlogPosts.Remove(blogPost);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}
    }
}
