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
        private HttpClient HttpClient = new HttpClient();

        // Convert BlogPostApiDto to BlogPostViewModel
        private static readonly Func<BlogPostApiDto, BlogPostViewModel> AsBlogPostViewModel =
            blogPostApiDto => new BlogPostViewModel
            {
                Id = blogPostApiDto.Id,
                Title = blogPostApiDto.Title,
                Content = blogPostApiDto.Content,
                ApplicationUserId = blogPostApiDto.ApplicationUserId
            };

        // Convert BlogPostViewModel to BlogPostApiDto
        private static readonly Func<BlogPostViewModel, BlogPostApiDto> AsBlogPostApiDto =
            blogPostViewModel => new BlogPostApiDto
            {
                Id = blogPostViewModel.Id,
                Title = blogPostViewModel.Title,
                Content = blogPostViewModel.Content,
                ApplicationUserId = blogPostViewModel.ApplicationUserId
            };

        // GET: BlogPosts
        public async Task<ActionResult> Index()
        {
            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetBlogPostsByApplicationUser,
                new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<IEnumerable<BlogPostApiDto>>(content);
                return View(model.Select(AsBlogPostViewModel));
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content")] BlogPostViewModel blogPostViewModel)
        {
            if (ModelState.IsValid)
            {
                var requestUri = UrlHelper.BuildRequestUri(RouteNames.PostBlogPostsByApplicationUser,
                    new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
                var httpResponseMessage = await HttpClient.PostAsJsonAsync(requestUri, AsBlogPostApiDto(blogPostViewModel));

                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                // an error occurred => here you could log the content returned by the remote server
                return Content("An error occurred: " + content);
            }

            return View(blogPostViewModel);
        }

        // GET: MvcBlogPosts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetBlogPost, new {id}, Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<BlogPostApiDto>(content);

                if (model == null)
                {
                    return HttpNotFound();
                }

                return View(AsBlogPostViewModel(model));
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
        }

        // POST: MvcBlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,ApplicationUserId")] BlogPostViewModel blogPostViewModel)
        {
            if (ModelState.IsValid)
            {
                var requestUri = UrlHelper.BuildRequestUri(RouteNames.PutBlogPost,
                    new { id = blogPostViewModel.Id }, Request.Url, Url);
                var httpResponseMessage = await HttpClient.PutAsJsonAsync(requestUri, AsBlogPostApiDto(blogPostViewModel));

                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                // an error occurred => here you could log the content returned by the remote server
                return Content("An error occurred: " + content);
            }

            return View(blogPostViewModel);
        }

        // GET: MvcBlogPosts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetBlogPost, new { id }, Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<BlogPostApiDto>(content);

                if (model == null)
                {
                    return HttpNotFound();
                }

                return View(AsBlogPostViewModel(model));
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
        }

        // POST: MvcBlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var requestUri = UrlHelper.BuildRequestUri(RouteNames.DeleteBlogPost, new { id }, Request.Url, Url);
            var httpResponseMessage = await HttpClient.DeleteAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
