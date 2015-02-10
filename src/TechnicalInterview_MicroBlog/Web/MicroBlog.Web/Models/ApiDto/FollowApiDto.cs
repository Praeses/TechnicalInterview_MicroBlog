namespace MicroBlog.Web.Models.ApiDto
{
    public class FollowApiDto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string ApplicationUserIdFollowed { get; set; }
    }
}