using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.ScheduledTasks;

namespace mobSocial.Services.ScheduledTasks
{
    public class ScheduledTaskService : BaseEntityService<ScheduledTask>, IScheduledTaskService
    {
        public ScheduledTaskService(IDataRepository<ScheduledTask> dataRepository) : base(dataRepository)
        {
        }
    }
}
