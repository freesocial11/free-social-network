using System;
using System.Collections.Generic;
using System.Web;

namespace mobSocial.Core
{
    public class WebHelper
    {
        /// <summary>
        /// Parses a url for rendering
        /// </summary>
        /// <returns></returns>
        public static string GetUrlFromPath(string path, string rootDomain = "")
        {
            //we need to see if the path is relative or absolute
            if (path.StartsWith("~"))
            {
                //it's a relative path to server
                return rootDomain + path.Substring(1);
            }
            //it may be an absolute url
            return rootDomain + path;
        }

        /// <summary>
        /// Gets the client's ip address
        /// </summary>
        /// <returns></returns>
        public static string GetClientIpAddress()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }
        /// <summary>
        /// Parses a url and returns a uri object
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Uri ParseUrl(string url, IDictionary<string, string> parameters = null)
        {
            try
            {
                var builder = new UriBuilder(url);
                if (parameters != null)
                {
                    var queryParams = HttpUtility.ParseQueryString(string.Empty);
                    foreach (var p in parameters)
                        queryParams[p.Key] = p.Value;

                    builder.Query = queryParams.ToString();
                }
                return builder.Uri;
            }
            catch (System.Exception)
            {
                return null;
            }
            

        }
    }
}