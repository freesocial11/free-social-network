using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using FluentScheduler;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Tasks;
using mobSocial.Data.Database;
using mobSocial.Data.Entity.ScheduledTasks;

namespace mobSocial.Services.ScheduledTasks
{
    public class TaskManager : ITaskManager
    {
        private readonly IScheduledTaskService _scheduledTaskService;

        public TaskManager(IScheduledTaskService scheduledTaskService)
        {
            _scheduledTaskService = scheduledTaskService;
        }

        public void Start(Type[] availableTasksTypes)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return;
            var container = mobSocialEngine.ActiveEngine.IocContainer;
            IList<ScheduledTask> scheduledTasks;
            //because dbcontext resolves per web request and the per request scope hasn't yet started here,
            //we'll have to fake open it, otherwise database context won't be resolved
            using (container.OpenScope(Reuse.WebRequestScopeName))
            {
                //get the scheduled tasks which are enabled
                scheduledTasks = _scheduledTaskService.Get(x => x.Enabled).ToList();
            }

            if (!scheduledTasks.Any())
                return;
            var registry = new Registry();
            //add each to the scheduler
            foreach (var sTask in scheduledTasks)
            {
                var taskType = availableTasksTypes.FirstOrDefault(x => x.FullName == sTask.SystemName);
                if (taskType != null)
                {
                    if (sTask.Seconds < 30)
                        sTask.Seconds = 30;
                    //schedule the task
                    registry.Schedule(() =>
                    {
                        using (var task = (ITask)Activator.CreateInstance(taskType))
                        {
                            try
                            {
                                sTask.LastStartDateTime = DateTime.UtcNow;
                                //because dbcontext resolves per web request and it won't be available for background tasks,
                                //we'll have to fake open it, otherwise database context won't be resolved
                                using (container.OpenScope(Reuse.WebRequestScopeName))
                                {
                                    task.Run();
                                }
                                sTask.LastSuccessDateTime = DateTime.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                //check if task should be stopped
                                if (sTask.StopOnError)
                                {
                                    sTask.Enabled = false;
                                    sTask.LastEndDateTime = DateTime.UtcNow;
                                }
                            }
                            finally
                            {
                                //update the task
                                _scheduledTaskService.Update(sTask);
                            }
                        }
                    }).ToRunEvery(sTask.Seconds).Seconds();

                }
            }

            //initialize the jobmanager
            JobManager.Initialize(registry);
        }
    }
}