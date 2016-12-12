using mobSocial.Data.Entity.Notifications;
using mobSocial.WebApi.Configuration.SignalR.Hubs;
using mobSocial.WebApi.Extensions.ModelExtensions;

namespace mobSocial.WebApi.Configuration.SignalR.Engines
{
    public class NotificationEngine : MobSocialHubEngine<Notification, NotificationHub>
    {
        public override void EntityUpdated(Notification entity)
        {
            //do nothing
        }

        public override void EntityDeleted(Notification entity)
        {
            //do nothing
        }

        public override void EntityInserted(Notification entity)
        {
            HubContext.Clients.User(entity.UserId.ToString()).notify(entity.ToModel());
        }
    }
}