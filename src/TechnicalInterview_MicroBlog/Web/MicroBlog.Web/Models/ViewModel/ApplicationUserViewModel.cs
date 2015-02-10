using System.ComponentModel;

namespace MicroBlog.Web.Models.ViewModel
{
    public class ApplicationUserViewModel
    {
        public string ApplicationUserId { get; set; }
        [DisplayName("Blogger's Name")]
        public string UserName { get; set; }
        public bool Follow { get; set; }
    }
}