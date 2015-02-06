using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBlog.Web.Models.Db
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}