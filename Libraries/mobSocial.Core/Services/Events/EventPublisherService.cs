using DryIoc;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Core.Services.Events
{
    public class EventPublisherService : IEventPublisherService
    {
        public void Publish<T>(T entity, EventType eventType) where T : BaseEntity
        {
            var eventConsumers = mobSocialEngine.ActiveEngine.IocContainer.ResolveMany<IEventConsumer<T>>();
            //first find out all the consumers
            foreach (var ec in eventConsumers)
            {
                switch (eventType)
                {
                    case EventType.Insert:
                        ec.EntityInserted(entity);
                        break;
                    case EventType.Update:
                        ec.EntityUpdated(entity);
                        break;
                    case EventType.Delete:
                        ec.EntityDeleted(entity);
                        break;
                }
            }


        }
    }
}