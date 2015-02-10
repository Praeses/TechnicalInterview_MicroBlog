namespace MicroBlog.Web.Models.ApiDto
{
    public class ApplicationUserApiDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool Follow { get; set; }
    }
}