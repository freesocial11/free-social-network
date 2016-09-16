using System;
using System.Linq;
using System.Web.Http;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.VerboseReporter;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Configuration.Mvc
{
    public abstract class RootApiController : ApiController
    {
        protected RootApiController()
        {
            VerboseReporter = mobSocialEngine.ActiveEngine.Resolve<IVerboseReporterService>();
        }
       
        protected IVerboseReporterService VerboseReporter;

        public IHttpActionResult Response(dynamic obj)
        {
            return Json(obj);
        }

        public IHttpActionResult RespondSuccess()
        {
            return RespondSuccess(null);
        }
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

        public IHttpActionResult RespondSuccess(string successMessage, string contextName, dynamic additionalData = null)
        {
            VerboseReporter.ReportSuccess(successMessage, contextName);
            return RespondFailure(additionalData);
        }

        public IHttpActionResult RespondFailure()
        {
            return RespondFailure(null);
        }

        public IHttpActionResult RespondFailure(string errorMessage, string contextName, dynamic additionalData = null)
        {
            VerboseReporter.ReportError(errorMessage, contextName);
            return RespondFailure(additionalData);
        }

        public IHttpActionResult RespondFailure(dynamic additionalData)
        {
            return Response(new RootResponseModel() {
                Success = false,
                ErrorMessages = VerboseReporter.GetErrorsList(),
                Messages = VerboseReporter.GetSuccessList(),
                ResponseData = additionalData
            });
        }

        
        public IHttpActionResult RespondRedirect(Uri redirectUrl)
        {
            return Redirect(redirectUrl);
        }

        public IHttpActionResult RespondRedirect(string redirectUrl)
        {
            return Redirect(redirectUrl);
        }

        protected new IHttpActionResult BadRequest()
        {
            return base.BadRequest(string.Join(",", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))));
        }
    }
}