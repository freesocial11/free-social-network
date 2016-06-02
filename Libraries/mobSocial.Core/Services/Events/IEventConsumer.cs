using mobSocial.Core.Data;

namespace mobSocial.Core.Services.Events
{
    /// <summary>
    /// Specifies an event consumer for the specified entity
    /// </summary>
    public interface IEventConsumer<T> where T: BaseEntity
    {
        void EntityInserted(T entity);

        void EntityUpdated(T entity);

        void EntityDeleted(T entity);
    }
}