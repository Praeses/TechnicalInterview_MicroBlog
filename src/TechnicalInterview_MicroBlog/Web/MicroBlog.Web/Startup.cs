using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MicroBlog.Web.Startup))]
namespace MicroBlog.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
