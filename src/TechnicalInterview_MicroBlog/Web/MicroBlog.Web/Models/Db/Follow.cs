using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBlog.Web.Models.Db
{
    public class Follow
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserIdFollowed { get; set; }
        [ForeignKey("ApplicationUserIdFollowed")]
        public virtual ApplicationUser ApplicationUserFollowed { get; set; }
    }
}