using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Routing;
using DryIoc;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Provider;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using mobSocial.Tests.Mock;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Mvc.Models;
using NUnit.Framework;

namespace mobSocial.Tests.Setup
{
    public class MobSocialTestCase
    {
        protected const string Salt = "x123456789x";

        protected DatabaseContext DatabaseContext;

        [TearDown]
        public void TearDown()
        {
            HttpContext.Current = null;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            //a quick hack to load all assemblies so that we get all the services available for testing
            AssemblyLoader.LoadBinDirectoryAssemblies();

            //fake the http context
            HttpContext.Current = MockHelper.GetMockedHttpContext();

            //start the engine in test mode
            mobSocialEngine.ActiveEngine.Start(true);

            //setup mock paraemters
            MockHelper.SetupMockParameters();

            var connectionString = GetTestConnectionString();
            
            var container = mobSocialEngine.ActiveEngine.IocContainer;

            //override database access to sqlce for faster testing
            container.Register<IDatabaseProvider>(
                made: Made.Of(() => new SqlCeDatabaseProvider()), reuse: Reuse.Singleton,
                ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            DatabaseContext = new DatabaseContext(connectionString);

            container.RegisterInstance(typeof(IDatabaseContext), DatabaseContext,
                ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            //recreate database
            DatabaseContext.Database.Delete();
            
            DatabaseContext.Database.Create();
            //Resolve<IInstallationService>().Install(GetTestConnectionString(), "System.Data.SqlServerCe.4.0");

        }
        protected string GetTestConnectionString()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            return "Data Source=" + (Path.GetDirectoryName(executingAssembly.Location)) + @"\\Tests.Db.sdf;Persist Security Info=False";
        }

        public static T GetValueFromJsonResult<T>(IHttpActionResult actionResult, string propertyName)
        {
            return GetValueFromJsonResult<T>((JsonResult<RootResponseModel>)actionResult, propertyName);
        }

        public static T GetValueFromJsonResult<T>(JsonResult<RootResponseModel> jsonResult, string propertyName)
        {
            var allProperties = jsonResult.Content.GetType().GetProperties();
            var property = allProperties.FirstOrDefault(p => string.CompareOrdinal(p.Name, propertyName) == 0);

            if (null == property)
                throw new ArgumentException("propertyName not found", nameof(propertyName));
            return (T)property.GetValue(jsonResult.Content, null);
        }

        protected T Resolve<T>(params object[] argumentsForConstructor) where T: class
        {
            return mobSocialEngine.ActiveEngine.IocContainer.Resolve<T>();
        }

        protected T ResolveController<T>() where T : class
        {
            var resolvedController = Resolve<T>() as RootApiController;
            if (resolvedController != null)
            {
                resolvedController.Url = MockHelper.MockedUrlHelper;
                resolvedController.Configuration = MockHelper.MockedConfiguration;
                resolvedController.Configuration.EnsureInitialized();
                
            }
            return resolvedController as T;
        }

        protected IDataRepository<T> ResolveRepository<T>() where T : BaseEntity
        {
            return mobSocialEngine.ActiveEngine.Resolve<IDataRepository<T>>();
        }

        /// <summary>
        /// Registers web api controller for dependency management. Call this in OneTimeSetup method to use in subsequent tests
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void RegisterController<T>()
        {
            var container = mobSocialEngine.ActiveEngine.IocContainer;
            if (!container.IsRegistered(typeof(T)))
                container.Register(typeof(T), setup: DryIoc.Setup.With(allowDisposableTransient: true));
        }

        protected void SaveEntity<T>(T entity) where T : BaseEntity
        {
            DatabaseContext.Set<T>().Add(entity);
            DatabaseContext.SaveChanges();
        }

        protected T SaveAndLoadEntity<T>(T entity) where T : BaseEntity
        {
            SaveEntity(entity);

            var retrieved = DatabaseContext.Set<T>().Find(entity.Id);
            return retrieved;

        }

        protected void Login()
        {
            //insert test user
            var testUser = GetTestUser();
            var userService = Resolve<IUserService>();
            //insert user
            userService.Insert(testUser);
            //sign in this
            ApplicationContext.Current.SignIn("test@example.com", false);
        }

        protected User GetTestUser(bool invalid = false)
        {
            if (invalid)
            {
                return new User();
            }
            var cryptographyService = Resolve<ICryptographyService>();
            
            return new User() {
                FirstName = "First",
                LastName = "Last",
                Active = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Guid = Guid.NewGuid(),
                Email = "test@example.com",
                LastLoginDate = DateTime.Now,
                Password =cryptographyService.GetHashedPassword("askforit", Salt, PasswordFormat.Sha256Hashed),
                PasswordFormat = PasswordFormat.Sha256Hashed,
                PasswordSalt = Salt,
                LastLoginIpAddress = "127.0.0.1",
                IsSystemAccount = false,
                Remarks = "random text",
                UserName = "firstlast"
            };
        }
    }
}