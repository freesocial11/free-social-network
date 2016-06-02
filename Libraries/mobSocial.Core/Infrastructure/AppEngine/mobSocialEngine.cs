using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DryIoc;
using DryIoc.Mvc;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.DependencyManagement;
using mobSocial.Core.Infrastructure.Media;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Core.Plugins;
using mobSocial.Core.Startup;

namespace mobSocial.Core.Infrastructure.AppEngine
{
    public class mobSocialEngine : IAppEngine
    {
        public Container IocContainer { get; private set; }

        public IList<PictureSize> PictureSizes { get; private set; }

        public T Resolve<T>(bool returnDefaultIfNotResolved = false) where T : class
        {
            return IocContainer.Resolve<T>(returnDefaultIfNotResolved ? IfUnresolved.ReturnDefault : IfUnresolved.Throw);
        }

        public T RegisterAndResolve<T>(object instance = null, bool instantiateIfNull = true) where T : class
        {
            if (instance == null)
                if (instantiateIfNull)
                    instance = Activator.CreateInstance<T>();
                else
                {
                    throw new mobSocialException("Can't register a null instance");
                }
            var typedInstance = Resolve<T>(true);
            if (typedInstance == null)
                IocContainer.RegisterInstance(instance, new SingletonReuse());
            return instance as T;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(bool testMode = false)
        {
            //setup ioc container
            SetupContainer();

            //setup dependency resolvers
            SetupDependencies(testMode);

            if (!testMode)
            {
                //run startup tasks
                RunStartupTasks();
            }

            //setup picture sizes
            SetupPictureSizes(testMode);

        }

        public void SetupContainer()
        {
            IocContainer = new Container();
        }

        private void SetupDependencies(bool testMode = false)
        {
            //first the self
            IocContainer.Register<IAppEngine, mobSocialEngine>(Reuse.Singleton);

            //now the other dependencies by other plugins or system
            var dependencies = TypeFinder.ClassesOfType<IDependencyRegistrar>();
            //create instances for them
            var dependencyInstances = dependencies.Select(dependency => (IDependencyRegistrar)Activator.CreateInstance(dependency)).ToList();
            //reorder according to priority
            dependencyInstances = dependencyInstances.OrderBy(x => x.Priority).ToList();

            foreach (var di in dependencyInstances)
                //register individual instances in that order
                di.RegisterDependencies(IocContainer);

            //and it's resolver
            if (!testMode)
                IocContainer.WithMvc();
        }

        private void RunStartupTasks()
        {
            var startupTasks = TypeFinder.ClassesOfType<IStartupTask>();
            var tasks =
                startupTasks.Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask)).ToList();

            //reorder according to prioiryt
            tasks = tasks.OrderBy(x => x.Priority).ToList();

            foreach (var task in tasks)
                task.Run();
            ;

        }

        private void SetupPictureSizes(bool testMode = false)
        {
            PictureSizes = new List<PictureSize>();
            if (testMode)
                return;
            var allPictureSizeRegistrars = TypeFinder.ClassesOfType<IPictureSizeRegistrar>();
            var allPictureSizeInstances =
                allPictureSizeRegistrars.Select(x => (IPictureSizeRegistrar)Activator.CreateInstance(x));

            foreach (var sizeInstance in allPictureSizeInstances)
                sizeInstance.RegisterPictureSize(PictureSizes);
        }

        private void InstallSystemPlugins()
        {
            var allPlugins = TypeFinder.ClassesOfType<IPlugin>();

            var systemPlugins =
                allPlugins.Where(x => (x as IPlugin).IsSystemPlugin)
                    .Select(plugin => (IPlugin)Activator.CreateInstance(plugin))
                    .ToList();

            //run the install method
            foreach (var plugin in systemPlugins)
            {
                if (!PluginEngine.IsInstalled(plugin.PluginInfo))
                    plugin.Install();
            }

        }

        public static mobSocialEngine ActiveEngine
        {
            get { return Singleton<mobSocialEngine>.Instance; }
        }
    }
}