using System;
using System.Linq;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Core.Startup;
using Owin;

namespace mobSocial.WebApi
{
    /// <summary>
    /// Class to run the startup owin tasks
    /// </summary>
    public static class OwinStartupManager
    {
        public static void RunAllOwinConfigurations(IAppBuilder app)
        {
            //first get all the assemblies which implement IOwinStartup
            var owinStartupTasks = TypeFinder.ClassesOfType<IOwinStartupTask>();

            var tasks =
                owinStartupTasks.Select(startupTask => (IOwinStartupTask)Activator.CreateInstance(startupTask)).ToList();

            //reorder according to priority
            tasks = tasks.OrderBy(x => x.Priority).ToList();

            foreach (var task in tasks)
                task.Configuration(app);
        }
    }
}