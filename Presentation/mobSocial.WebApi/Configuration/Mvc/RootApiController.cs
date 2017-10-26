using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.VerboseReporter;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Configuration.Mvc.Results;

namespace mobSocial.WebApi.Configuration.Mvc
{
    public abstract class RootApiController : ApiController
    {
        protected RootApiController()
        {
            VerboseReporter = mobSocialEngine.ActiveEngine.Resolve<IVerboseReporterService>();
        }
       
        protected IVerboseReporterService VerboseReporter;
        [NonAction]
        public IHttpActionResult Response(dynamic obj)
        {
            return Json(obj);
        }
        [NonAction]
        public IHttpActionResult RespondSuccess()
        {
            return RespondSuccess(null);
        }
        [NonAction]
        public IHttpActionResult RespondSuccess(dynamic additionalData)
        {
            return Response(new RootResponseModel()
            {
                Success = true,
                ErrorMessages = VerboseReporter.GetErrorsList(),
                Messages = VerboseReporter.GetSuccessList(),
                ResponseData = additionalData
            });
        }
        [NonAction]
        public IHttpActionResult RespondSuccess(string successMessage, string contextName, dynamic additionalData = null)
        {
            VerboseReporter.ReportSuccess(successMessage, contextName);
            return RespondSuccess(additionalData);
        }
        [NonAction]
        public IHttpActionResult RespondFailure()
        {
            return RespondFailure(null);
        }
        [NonAction]
        public IHttpActionResult RespondFailure(string errorMessage, string contextName, dynamic additionalData = null)
        {
            VerboseReporter.ReportError(errorMessage, contextName);
            return RespondFailure(additionalData);
        }
        [NonAction]
        public IHttpActionResult RespondFailure(dynamic additionalData)
        {
            return Response(new RootResponseModel() {
                Success = false,
                ErrorMessages = VerboseReporter.GetErrorsList(),
                Messages = VerboseReporter.GetSuccessList(),
                ResponseData = additionalData
            });
        }

        [NonAction]
        public IHttpActionResult RespondRedirect(Uri redirectUrl)
        {
            return Redirect(redirectUrl);
        }
        [NonAction]
        public IHttpActionResult RespondRedirect(string redirectUrl)
        {
            return Redirect(redirectUrl);
        }
        [NonAction]
        protected new IHttpActionResult BadRequest()
        {
            return base.BadRequest(string.Join(",", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))));
        }
        [NonAction]
        public IHttpActionResult View(string view, object model = null)
        {
            return new HtmlActionResult(view, model);
        }

        public IHttpActionResult Respond(HttpStatusCode code)
        {
            return ResponseMessage(new HttpResponseMessage(code));
        }
    }
}