using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace MicroBlog.Web.Controllers
{
    public class MvcApplicationUsersController : Controller
    {
        private HttpClient HttpClient = new HttpClient();

        // Convert ApplicationUserApiDto to ApplicationUserViewModel
        private static readonly Func<ApplicationUserApiDto, ApplicationUserViewModel> AsApplicationUserViewModel =
            applicationUserApiDto => new ApplicationUserViewModel
            {
                ApplicationUserId = applicationUserApiDto.Id,
                UserName = applicationUserApiDto.UserName,
                Follow = applicationUserApiDto.Follow
            };

        // GET: MvcApplicationUsers
        public async Task<ActionResult> Index()
        {
            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetFollowableApplicationUsersByApplicationUser,
                new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<IEnumerable<ApplicationUserApiDto>>(content);
                return View(model.Select(AsApplicationUserViewModel));
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
        }

        //// GET: MvcApplicationUsers/Details/5
        //public async Task<ActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = await db.ApplicationUsers.FindAsync(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}

        //// GET: MvcApplicationUsers/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: MvcApplicationUsers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ApplicationUsers.Add(applicationUser);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(applicationUser);
        //}

        // GET: MvcApplicationUsers/Edit/aa95662b-e5c8-4225-a5cf-1c9b65492c01
        [Route("MvcApplicationUsers/Edit/{ApplicationUserId}")]
        public async Task<ActionResult> Edit(string applicationUserId)
        {
            if (applicationUserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var requestUri = UrlHelper.BuildRequestUri(RouteNames.GetFollowableApplicationUserByApplicationUser,
                new {applicationUserId = User.Identity.GetUserId(), followableApplicationUserId = applicationUserId},
                Request.Url, Url);
            var httpResponseMessage = await HttpClient.GetAsync(requestUri);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ApplicationUserApiDto>(content);

                if (model == null)
                {
                    return HttpNotFound();
                }

                return View(AsApplicationUserViewModel(model));
            }

            // an error occurred => here you could log the content returned by the remote server
            return Content("An error occurred: " + content);
        }

        // POST: MvcApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ApplicationUserId,UserName,Follow")] ApplicationUserViewModel applicationUserViewModel)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage httpResponseMessage;
                // Instead of doing this if/else post/delete technique, I could have implemented this by PUT'ing back an updated Followable ApplicationUser
                if (applicationUserViewModel.Follow) // If the current user wants to follow this ApplicationUser, then POST
                {
                    var requestUri = UrlHelper.BuildRequestUri(RouteNames.PostFollowsByApplicationUser,
                        new {applicationUserId = User.Identity.GetUserId()}, Request.Url, Url);

                    var followApiDto = new FollowApiDto
                    {
                        ApplicationUserIdFollowed = applicationUserViewModel.ApplicationUserId
                    };

                    httpResponseMessage = await HttpClient.PostAsJsonAsync(requestUri, followApiDto);
                }
                else // If the current user wants to UNfollow this ApplicationUser, then DELETE
                {
                    var requestUri = UrlHelper.BuildRequestUri(RouteNames.DeleteFollowsByApplicationUser,
                        new
                        {
                            applicationUserId = User.Identity.GetUserId(),
                            applicationUserIdFollowed = applicationUserViewModel.ApplicationUserId
                        }, Request.Url, Url);

                    httpResponseMessage = await HttpClient.DeleteAsync(requestUri);
                }

                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                // an error occurred => here you could log the content returned by the remote server
                return Content("An error occurred: " + content);
            }

            return View(applicationUserViewModel);
        }

        //// GET: MvcApplicationUsers/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = await db.ApplicationUsers.FindAsync(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}

        //// POST: MvcApplicationUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    ApplicationUser applicationUser = await db.ApplicationUsers.FindAsync(id);
        //    db.ApplicationUsers.Remove(applicationUser);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
