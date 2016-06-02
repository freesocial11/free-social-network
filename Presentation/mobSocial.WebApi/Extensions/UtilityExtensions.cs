using System;
using System.Web;

namespace mobSocial.WebApi.Extensions
{
    public static class UtilityExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request, bool pjaxOnly = false)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (pjaxOnly)
            {
                return ((request.Headers["X-PJAX"] != null));
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        public static bool IsNumeric(this string str)
        {
            float output;
            return float.TryParse(str, out output);
        }

        public static bool IsInteger(this string str)
        {
            int output;
            return int.TryParse(str, out output);
        }

       

       
    }
}
