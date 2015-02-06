using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MicroBlog.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Commenting-out because I want more fine-grained control over my URI's, so I'll use Attribute Routing
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
