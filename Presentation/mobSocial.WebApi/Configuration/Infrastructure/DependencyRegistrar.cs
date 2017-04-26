using System;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Web;
using DryIoc;
using mobSocial.Core.Caching;
using mobSocial.Core.Config;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.DependencyManagement;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Core.Services.Events;
using mobSocial.Data;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Provider;
using mobSocial.Services.Authentication;
using mobSocial.Services.Settings;
using mobSocial.Services.VerboseReporter;
using mobSocial.WebApi.Configuration.Database;
using mobSocial.WebApi.Configuration.Mvc.UI;
using mobSocial.WebApi.Configuration.SignalR.Providers;
using Microsoft.AspNet.SignalR;

namespace mobSocial.WebApi.Configuration.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDependencies(Container container)
        {
            //http context
            container.RegisterInstance<HttpContextBase>(new HttpContextWrapper(HttpContext.Current) as HttpContextBase, new SingletonReuse());

            //cache provider
            container.Register<ICacheProvider, HttpCacheProvider>(reuse: Reuse.Singleton);

            // settings register for access across app
            container.Register<IDatabaseSettings>(made: Made.Of(() => new DatabaseSettings()), reuse: Reuse.Singleton);

            //data provider : TODO: Use settings to determine the support for other providers
            container.Register<IDatabaseProvider>(made: Made.Of(() => new SqlServerDatabaseProvider()), reuse: Reuse.Singleton);

            //database context
            container.Register<IDatabaseContext>(made: Made.Of(() => DatabaseContextManager.GetDatabaseContext()), reuse: Reuse.InWebRequest);

            //and respositories
            container.Register(typeof(IDataRepository<>), typeof(EntityRepository<>), made: Made.Of(FactoryMethod.ConstructorWithResolvableArguments), reuse: Reuse.Singleton);

            var asm = AssemblyLoader.LoadBinDirectoryAssemblies();

            //services
            //to register services, we need to get all types from services assembly and register each of them;
            var serviceAssembly = asm.First(x => x.FullName.Contains("mobSocial.Services"));
            var serviceTypes = serviceAssembly.GetTypes().
                Where(type => type.IsPublic && // get public types 
                              !type.IsAbstract && // which are not interfaces nor abstract
                              type.GetInterfaces().Length != 0);// which implementing some interface(s)

            container.RegisterMany(serviceTypes, reuse: Reuse.Singleton);

            //we need a trasient reporter service rather than singleton
            container.Register<IVerboseReporterService, VerboseReporterService>(reuse: Reuse.InWebRequest, ifAlreadyRegistered:IfAlreadyRegistered.Replace);

            //settings
            var allSettingTypes = TypeFinder.ClassesOfType<ISettingGroup>();
            foreach (var settingType in allSettingTypes)
            {
                var type = settingType;
                container.RegisterDelegate(type, resolver =>
                {
                    var instance = (ISettingGroup) Activator.CreateInstance(type);
                    resolver.Resolve<ISettingService>().LoadSettings(instance);
                    return instance;
                }, reuse: Reuse.Singleton);

            }
            //and ofcourse the page generator
            container.Register<IPageGenerator, PageGenerator>(reuse: Reuse.Singleton);

            //event publishers and consumers
            container.Register<IEventPublisherService, EventPublisherService>(reuse: Reuse.Singleton);
            //all consumers which are not interfaces
            //find all event consumer types
            var allConsumerTypes = asm.SelectMany(x =>
                {
                    try
                    {
                        return x.GetTypes();
                    }
                    catch
                    {
                        return new Type[0];
                    }
                })
                .Where(type => type.IsPublic && // get public types 
                type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventConsumer<>)) &&
                !type.IsAbstract);// which implementing some interface(s)
            //all consumers which are not interfaces
            container.RegisterMany(allConsumerTypes);

            //user id provider for SignalR
            container.Register<IUserIdProvider, SignalRUserIdProvider>();

            //register authentication service inwebrequest
            container.Register<IAuthenticationService, AuthenticationService>(reuse: Reuse.InWebRequest, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        }

        public int Priority
        {
            get { return -int.MaxValue; }
        }
    }
    
}