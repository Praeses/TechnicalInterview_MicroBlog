using System.Linq;
using System.Web.Http;
using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;

namespace MicroBlog.Web.Controllers
{
    public class ApiFollowedBlogPostController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/FollowedBlogPosts
        [Route("api/ApplicationUsers/{ApplicationUserId}/FollowedBlogPosts",
            Name = "GetFollowedBlogPostsByApplicationUser")]
        public IQueryable<FollowedBlogPostApiDto> GetFollowedBlogPostsByApplicationUser(string applicationUserId)
        {
            // I initially had the relatively meaningful range variable names u, f, fu, and b
            // ...but I'll use these instead just for fun...
            return
                from MR in db.Users
                join IO in db.Follows on MR.Id equals IO.ApplicationUserId
                join KW in db.Users on IO.ApplicationUserIdFollowed equals KW.Id
                join EE in db.BlogPosts on KW.Id equals EE.ApplicationUserId
                where MR.Id.Equals(applicationUserId)
                orderby EE.Id descending
                select new FollowedBlogPostApiDto
                {
                    ApplicationUserId = KW.Id,
                    UserName = KW.UserName,
                    BlogPostId = EE.Id,
                    Title = EE.Title,
                    Content = EE.Content
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