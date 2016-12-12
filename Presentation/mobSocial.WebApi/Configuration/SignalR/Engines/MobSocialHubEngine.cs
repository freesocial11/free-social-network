using mobSocial.Core.Data;
using mobSocial.Core.Services.Events;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace mobSocial.WebApi.Configuration.SignalR.Engines
{
    public abstract class MobSocialHubEngine<TEntity, THub> : IEventConsumer<TEntity> where TEntity : BaseEntity where THub : IHub
    {
        protected readonly IHubContext HubContext;

        protected MobSocialHubEngine()
        {
            HubContext = GlobalHost.ConnectionManager.GetHubContext<THub>();
        }

        public abstract void EntityDeleted(TEntity entity);

        public abstract void EntityInserted(TEntity entity);

        public abstract void EntityUpdated(TEntity entity);
    }
}