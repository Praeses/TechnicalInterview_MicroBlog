namespace MicroBlog.Web.Models.ViewModel
{
    public class FollowedBlogPostViewModel
    {
        public string ApplicationUserId { get; set; }
        public string UserName { get; set; }

        public int BlogPostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}