using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Extensions;

namespace mobSocial.WebApi.Configuration.Security.Attributes
{
    /// <summary>
    /// Specifies that current application must have provided scope to authorize the call
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scope { get; set; }

        public bool AllowForNullApplications { get; set; } = true;

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!ApplicationContext.Current.CurrentOAuthApplication.HasScope(Scope, AllowForNullApplications))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "{\"message\" : \"Application is not unauthorized to access that endpoint\"}");
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}