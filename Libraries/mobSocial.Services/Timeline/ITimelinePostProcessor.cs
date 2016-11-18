using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Timeline;

namespace mobSocial.Services.Timeline
{
    public interface ITimelinePostProcessor
    {
        void ProcessInlineTags(TimelinePost post);

        void ProcessInlineTags(Comment comment);
    }
}