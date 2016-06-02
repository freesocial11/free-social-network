using System;
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
            
        }
       
        protected IVerboseReporterService VerboseReporter => mobSocialEngine.ActiveEngine.Resolve<IVerboseReporterService>();

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

        public IHttpActionResult RespondFailure()
        {
            return RespondFailure(null);
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
    }
}