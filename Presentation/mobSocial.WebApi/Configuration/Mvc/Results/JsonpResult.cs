using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace mobSocial.WebApi.Configuration.Mvc.Results
{
    public class JsonpResult : JsonResult
    {

        public JsonpResult()
        {
        }



        public override void ExecuteResult(ControllerContext controllerContext)
        {
            if (controllerContext != null)
            {
                var response = controllerContext.HttpContext.Response;
                var request = controllerContext.HttpContext.Request;

                var callbackfunction = request["callback"];
                if (string.IsNullOrEmpty(callbackfunction))
                {
                    throw new Exception("Callback function name must be provided in the request!");
                }
                response.ContentType = "application/x-javascript";
                if (Data != null)
                {

                    var serializer = new JavaScriptSerializer();
                    response.Write($"{callbackfunction}({serializer.Serialize(this.Data)});");
                }
            }
        }
    }
}
