using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.Db;

namespace MicroBlog.Web.Controllers
{
    public class ApiApplicationUsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Convert ApplicationUser to ApplicationUserApiDto
        private static readonly Expression<Func<ApplicationUser, ApplicationUserApiDto>> AsApplicationUserApiDto =
            applicationUser => new ApplicationUserApiDto
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName
            };

        // GET: api/ApplicationUsers
        [Route("api/ApplicationUsers", Name = "GetApplicationUsers")]
        public IQueryable<ApplicationUserApiDto> GetApplicationUsers()
        {
            return db.Users.Select(AsApplicationUserApiDto);
        }

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/FollowableApplicationUsers/b2628677-dcd9-4416-b133-24d3f9637a18
        [Route("api/ApplicationUsers/{ApplicationUserId}/FollowableApplicationUsers/{FollowableApplicationUserId}", Name = "GetFollowableApplicationUserByApplicationUser")]
        [ResponseType(typeof(ApplicationUserApiDto))]
        public async Task<IHttpActionResult> GetFollowableApplicationUserByApplicationUser(string applicationUserId, string followableApplicationUserId)
        {
            ApplicationUserApiDto applicationUserApiDto = await db.Users
                .Where(b => b.Id == followableApplicationUserId)
                .Select(AsApplicationUserApiDto)
                .FirstOrDefaultAsync();

            if (applicationUserApiDto == null)
            {
                return NotFound();
            }

            applicationUserApiDto.Follow = (from follow in db.Follows
                where
                    follow.ApplicationUserId.Equals(applicationUserId) &&
                    follow.ApplicationUserIdFollowed.Equals(followableApplicationUserId)
                select follow).Any();

            return Ok(applicationUserApiDto);
        }

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/FollowableApplicationUsers
        [Route("api/ApplicationUsers/{ApplicationUserId}/FollowableApplicationUsers", Name = "GetFollowableApplicationUsersByApplicationUser")]
        public IEnumerable<ApplicationUserApiDto> GetFollowableApplicationUsersByApplicationUser(string applicationUserId)
        {
            // Get all ApplicationUsers except the current user (you can't follow yourself)
            var followableApplicationUsersByApplicationUser = db.Users
                .Where(b => !b.Id.Equals(applicationUserId))
                .Select(AsApplicationUserApiDto).ToList();

            // Get all Follows for the current user
            var followsByApplicationUser = (from follow in db.Follows
                where follow.ApplicationUserId.Equals(applicationUserId)
                select follow).ToList();

            // Flag each ApplicationUser that the current user follows
            foreach (var follow in followsByApplicationUser)
            {
                var followedApplicationUser =
                    followableApplicationUsersByApplicationUser.FirstOrDefault(
                        f => f.Id.Equals(follow.ApplicationUserIdFollowed));
                if (followedApplicationUser != null)
                {
                    followedApplicationUser.Follow = true;
                }
            }

            return followableApplicationUsersByApplicationUser;
        }
        
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
