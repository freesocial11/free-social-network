using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Configuration.Infrastructure;

namespace mobSocial.WebApi.Configuration.Security.Attributes
{
    /// <summary>
    /// Specifies tha logged in user must be administrator to access this area
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!ApplicationContext.Current.CurrentUser.IsAdministrator())
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized access");
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}