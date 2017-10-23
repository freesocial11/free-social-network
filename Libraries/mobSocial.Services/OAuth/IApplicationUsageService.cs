using mobSocial.Core.Services;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Services.OAuth
{
    public interface IApplicationUsageService : IBaseEntityService<ApplicationUsage>
    {
        void TrackUsage(int applicationId);

        bool IsUsageAllowed(int applicationId);
    }
}