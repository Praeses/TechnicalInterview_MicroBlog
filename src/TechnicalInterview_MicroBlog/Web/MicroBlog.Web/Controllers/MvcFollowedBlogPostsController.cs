using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MicroBlog.Web.Controllers
{
    public class MvcFollowedBlogPostsController : Controller
    {
        private HttpClient HttpClient = new HttpClient();

        // Convert FollowedBlogPostApiDto to FollowedBlogPostViewModel
        private static readonly Func<FollowedBlogPostApiDto, FollowedBlogPostViewModel> AsFollowedBlogPostViewModel =
            followedBlogPostApiDto => new FollowedBlogPostViewModel
            {
                ApplicationUserId = followedBlogPostApiDto.ApplicationUserId,
                UserName = followedBlogPostApiDto.UserName,
                BlogPostId = followedBlogPostApiDto.BlogPostId,
                Title = followedBlogPostApiDto.Title,
                Content = followedBlogPostApiDto.Content
            };

        // GET: MvcFollowedBlogPosts
        public async Task<ActionResult> Index()
        {
            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetFollowedBlogPostsByApplicationUser,
                new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<IEnumerable<FollowedBlogPostApiDto>>(content);
                return View(model.Select(AsFollowedBlogPostViewModel));
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
