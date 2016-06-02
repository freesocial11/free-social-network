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
    }
}