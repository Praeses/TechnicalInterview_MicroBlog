namespace MicroBlog.Web.Models.ApiDto
{
    public class FollowedBlogPostApiDto
    {
        public string ApplicationUserId { get; set; }
        public string UserName { get; set; }

        public int BlogPostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}