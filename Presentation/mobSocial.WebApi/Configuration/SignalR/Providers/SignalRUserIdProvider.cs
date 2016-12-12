using mobSocial.WebApi.Configuration.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace mobSocial.WebApi.Configuration.SignalR.Providers
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return ApplicationContext.Current.CurrentUser?.Id.ToString();
        }
    }
}