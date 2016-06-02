using mobSocial.Core.Data;

namespace mobSocial.Services.Timeline
{
    public interface ITimelineAutoPublisher
    {
        void Publish<T>(T entity, string postTypeName, int ownerId) where T: BaseEntity;
    }
}