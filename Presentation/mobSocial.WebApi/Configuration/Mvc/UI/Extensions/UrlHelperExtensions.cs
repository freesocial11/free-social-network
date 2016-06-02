using System;
using System.Web.Http.Routing;
using mobSocial.Core.Exception;
using UrlHelper = System.Web.Mvc.UrlHelper;

namespace mobSocial.WebApi.Configuration.Mvc.UI.Extensions
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Retrieves a particular routeurl from the global route table. Because the app is running inside OWIN host, we need to query route from the global routetable 
        /// instead of route from OWIN configuration
        /// </summary>
        /// <returns>The url of route</returns>
        public static string RouteAppUrl(this UrlHelper urlHelper, string routeName, dynamic routeValues)
        {
            if (routeValues == null)
                routeValues = new { };

            try
            {
                return urlHelper.RouteUrl(routeName, routeValues);
                // RouteTable.Routes.GetVirtualPath(null, new RouteValueDictionary(routeValues));
            }
            catch (Exception ex)
            {
                // ignored
            }
            //search in webapi routes
            IHttpRoute apiRoute;
            var vPath = WebApiConfig.Configuration.Routes.TryGetValue(routeName, out apiRoute);
            
            
            if (!vPath)
                throw new mobSocialException(string.Format("Can't find a route named {0} in the RouteTable", routeName));
            return apiRoute.RouteTemplate;
        }

        public static string RouteAppUrl(this UrlHelper urlHelper, string routeName)
        {
            return RouteAppUrl(urlHelper, routeName, null);
        }
    }
}