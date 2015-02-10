using System.ComponentModel.DataAnnotations;

namespace MicroBlog.Web.Models.ViewModel
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^.*Mike Rowe.*$", ErrorMessage = "The title of your blog post must contain \"Mike Rowe\". Don't try to change the subject!")]
        public string Title { get; set; }
        [Required]
        [RegularExpression(@"^.*Mike.*$", ErrorMessage = "The content of your blog post must contain \"Mike\". Afterall...you did chose to include his name in the Title...")]
        public string Content { get; set; }
        public string ApplicationUserId { get; set; }
    }
}