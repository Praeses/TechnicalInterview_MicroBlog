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
            return
                from u in db.Users
                join f in db.Follows on u.Id equals f.ApplicationUserId
                join fu in db.Users on f.ApplicationUserIdFollowed equals fu.Id
                join b in db.BlogPosts on fu.Id equals b.ApplicationUserId
                where u.Id.Equals(applicationUserId)
                orderby b.Id descending
                select new FollowedBlogPostApiDto
                {
                    ApplicationUserId = fu.Id,
                    UserName = fu.UserName,
                    BlogPostId = b.Id,
                    Title = b.Title,
                    Content = b.Content
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