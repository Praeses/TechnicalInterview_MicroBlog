using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;

namespace MicroBlog.Web.Controllers
{
    public class ApiApplicationUsersController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/FollowableApplicationUsers/b2628677-dcd9-4416-b133-24d3f9637a18
        [Route("api/ApplicationUsers/{ApplicationUserId}/FollowableApplicationUsers/{FollowableApplicationUserId}",
            Name = RouteNames.GetFollowableApplicationUserByApplicationUser)]
        [ResponseType(typeof (ApplicationUserApiDto))]
        public async Task<IHttpActionResult> GetFollowableApplicationUserByApplicationUser(string applicationUserId,
            string followableApplicationUserId)
        {
            IQueryable<ApplicationUserApiDto> followableApplicationUserByApplicationUserQuery =
                from u in GetFollowableApplicationUsersByApplicationUser(applicationUserId)
                where u.Id == followableApplicationUserId
                select u;

            ApplicationUserApiDto applicationUserApiDto =
                await followableApplicationUserByApplicationUserQuery.FirstOrDefaultAsync();

            if (applicationUserApiDto == null)
            {
                return NotFound();
            }

            return Ok(applicationUserApiDto);
        }

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/FollowableApplicationUsers
        [Route("api/ApplicationUsers/{ApplicationUserId}/FollowableApplicationUsers",
            Name = RouteNames.GetFollowableApplicationUsersByApplicationUser)]
        public IQueryable<ApplicationUserApiDto> GetFollowableApplicationUsersByApplicationUser(string applicationUserId)
        {
            // How to: Perform Left Outer Joins https://msdn.microsoft.com/en-us/library/bb397895.aspx
            // join on multiple conditions (hey this "Anders Hejlsberg" fellow seems pretty smart) ;) https://social.msdn.microsoft.com/Forums/en-US/e09b50af-5ab4-4cfb-9617-ef2be339c17f/join-on-multiple-conditions
            // need to assign a name to each type in the anonymous type http://stackoverflow.com/a/5568354/1097016
            // This linq has become just messy enough that I'm almost tempted to go back to the previous more imperative style.
            return
                from u in db.Users
                // Left Outer Join to get all users that are followed by the current user
                join f in db.Follows
                    on new {a = u, b = applicationUserId} equals
                    new {a = f.ApplicationUserFollowed, b = f.ApplicationUserId} into gj
                from subf in gj.DefaultIfEmpty()
                // Exclude the current user (the data model won't prevent you from following yourself, but logically let's prevent that from happening here)
                where (!u.Id.Equals(applicationUserId))
                select
                    new ApplicationUserApiDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Follow = subf != null // Flag each ApplicationUser that the current user follows
                    };
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