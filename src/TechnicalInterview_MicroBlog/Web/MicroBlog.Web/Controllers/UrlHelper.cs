using System;

namespace MicroBlog.Web.Controllers
{
    public class UrlHelper
    {
        // System.Web.Mvc.UrlHelper methods RouteUrl() and HttpRouteUrl() don't seem to play well with Attribute Routing. Rolling my own!
        public static string BuildRequestUri(string routeName, object routeValues, Uri baseRequestUrl,
            System.Web.Mvc.UrlHelper urlHelper)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = baseRequestUrl.Scheme,
                Port = baseRequestUrl.Port,
                Host = baseRequestUrl.Host,
                Path =
                    urlHelper.HttpRouteUrl(routeName,
                        routeValues)
            };
            string requestUri = uriBuilder.Uri.AbsoluteUri;
            return requestUri;
        }
    }
}