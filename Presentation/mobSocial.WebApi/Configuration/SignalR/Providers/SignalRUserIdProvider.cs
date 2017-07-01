using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.WebApi.Configuration.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace mobSocial.WebApi.Configuration.SignalR.Providers
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
            {
                return ApplicationContext.Current.CurrentUser?.Id.ToString();
            }
        }
    }
}