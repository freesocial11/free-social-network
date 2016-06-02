// 

using System.Web.Http;

namespace mobSocial.Tests.Setup.Extensions
{
    public static class HttpActionResultExtensions
    {
        public static T GetValue<T>(this IHttpActionResult result, string parameterName)
        {
            return MobSocialTestCase.GetValueFromJsonResult<T>(result, parameterName);
        }
    }
}