namespace MicroBlog.Web.Models.ViewModel
{
    public class BlogPostViewModel
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ApplicationUserId { get; set; }
    }
}