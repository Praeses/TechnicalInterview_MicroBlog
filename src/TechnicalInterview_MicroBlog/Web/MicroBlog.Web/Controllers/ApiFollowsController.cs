using MicroBlog.Web.Models;
using MicroBlog.Web.Models.ApiDto;
using MicroBlog.Web.Models.Db;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace MicroBlog.Web.Controllers
{
    public class ApiFollowsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Convert FollowApiDto to Follow
        private static readonly Func<FollowApiDto, Follow> AsFollow = followApiDto => new Follow
        {
            Id = followApiDto.Id,
            ApplicationUserId = followApiDto.ApplicationUserId,
            ApplicationUserIdFollowed = followApiDto.ApplicationUserIdFollowed
        };

        // Wrap linq expression in Func, compile, and invoke. This allows me to define the mapping in one place.
        // A bit odd...but at least it's DRY.
        private static readonly Func<Follow, FollowApiDto> AsFollowApiDto = follow => AsFollowApiDtoExpression.Compile().Invoke(follow);

        // Convert Follow to FollowApiDto
        private static readonly Expression<Func<Follow, FollowApiDto>> AsFollowApiDtoExpression =
            follow => new FollowApiDto
            {
                Id = follow.Id,
                ApplicationUserId = follow.ApplicationUserId,
                ApplicationUserIdFollowed = follow.ApplicationUserIdFollowed
            };

        // GET: api/ApiFollows
        [Route("api/Follows")]
        public IQueryable<FollowApiDto> GetFollows()
        {
            return db.Follows.Select(AsFollowApiDtoExpression);
        }

        // GET: api/ApiFollows/5
        [Route("api/Follows/{id:int}", Name = RouteNames.GetFollow)]
        [ResponseType(typeof(FollowApiDto))]
        public async Task<IHttpActionResult> GetFollow(int id)
        {
            FollowApiDto followApiDto = await db.Follows
                .Where(f => f.Id == id)
                .Select(AsFollowApiDtoExpression)
                .FirstOrDefaultAsync();

            if (followApiDto == null)
            {
                return NotFound();
            }

            return Ok(followApiDto);
        }

        // POST: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/Follows
        [Route("api/ApplicationUsers/{ApplicationUserId}/Follows", Name = RouteNames.PostFollowsByApplicationUser)]
        [ResponseType(typeof(FollowApiDto))]
        public async Task<IHttpActionResult> PostFollowsByApplicationUser(string applicationUserId, FollowApiDto followApiDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var follow = AsFollow(followApiDto);
            follow.ApplicationUserId = applicationUserId; // Set ApplicationUserId to applicationUserId URI segment value
            db.Follows.Add(follow);
            await db.SaveChangesAsync();

            return CreatedAtRoute(RouteNames.GetFollow, new { id = follow.Id }, AsFollowApiDto(follow));
        }

        // DELETE: api/ApplicationUsers/aa95662b-e5c8-4225-a5cf-1c9b65492c01/Follows/b2628677-dcd9-4416-b133-24d3f9637a18
        [Route("api/ApplicationUsers/{ApplicationUserId}/Follows/{ApplicationUserIdFollowed}", Name = RouteNames.DeleteFollowsByApplicationUser)]
        [ResponseType(typeof(FollowApiDto))]
        public async Task<IHttpActionResult> DeleteFollowsByApplicationUser(string applicationUserId, string applicationUserIdFollowed)
        {
            Follow follow =
                await
                    db.Follows.Where(
                        f =>
                            f.ApplicationUserId.Equals(applicationUserId) &&
                            f.ApplicationUserIdFollowed.Equals(applicationUserIdFollowed)).FirstOrDefaultAsync();
            if (follow == null)
            {
                return NotFound();
            }

            db.Follows.Remove(follow);
            await db.SaveChangesAsync();

            return Ok(AsFollowApiDto(follow));
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