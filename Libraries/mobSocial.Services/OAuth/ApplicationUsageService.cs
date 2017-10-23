using System;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Services.OAuth
{
    public class ApplicationUsageService : BaseEntityService<ApplicationUsage>, IApplicationUsageService
    {
        private readonly IApplicationService _applicationService;
        public ApplicationUsageService(IDataRepository<ApplicationUsage> dataRepository, IApplicationService applicationService) : base(dataRepository)
        {
            _applicationService = applicationService;
        }


        public void TrackUsage(int applicationId)
        {
            var now = DateTime.UtcNow;
            var appUsage = FirstOrDefault(x => x.ApplicationId == applicationId) ?? new ApplicationUsage()
            {
                ApplicationId = applicationId,
                LastRequested = now
            };
            //we should reset the count every hour
            if (now.Subtract(appUsage.LastRequested).TotalHours >= 1)
                appUsage.UsageCount = 0;

            appUsage.UsageCount++;
            appUsage.LastRequested = DateTime.UtcNow;
            if(appUsage.Id == 0)
                Insert(appUsage);
            else
                Update(appUsage);
        }

        public bool IsUsageAllowed(int applicationId)
        {
            //get the application
            var application = _applicationService.Get(applicationId);
            var maxAllowedUsagePerHour = application.RequestLimitPerHour;

            //get the app usage
            var appUsage = FirstOrDefault(x => x.ApplicationId == applicationId);
            if (appUsage == null)
                return true;

            var now = DateTime.UtcNow;
            if (now.Subtract(appUsage.LastRequested).TotalHours < 1 && appUsage.UsageCount >= maxAllowedUsagePerHour)
            {
                return false;
            }
            return true;
        }
    }
}