using System.Web.Mvc;
using mobSocial.WebApi.Configuration.Mvc.Results;

namespace mobSocial.WebApi.Extensions
{
    public static class JsonResultExtensions
    {
        public static JsonpResult ToJsonp(this JsonResult json)
        {
            return new JsonpResult { ContentEncoding = json.ContentEncoding, ContentType = json.ContentType, Data = json.Data, JsonRequestBehavior = json.JsonRequestBehavior };
        }
    }
}
