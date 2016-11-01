using System.Web;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.Services.Installation;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.Installation;

namespace mobSocial.WebApi.Controllers
{
    public class InstallController : RootApiController
    {
        private readonly IInstallationService _installationService;

        public InstallController(IInstallationService installationService)
        {
            _installationService = installationService;
        }


        [Route("install")]
        [HttpPost]
        public IHttpActionResult Install(InstallationRequestModel model)
        {
            if (!ModelState.IsValid)
                return Response(new { Success = false, Message = "Insufficient data sent to complete installation" });

            var areTableInstalled = DatabaseManager.IsDatabaseInstalled();

            if (areTableInstalled)
                return Response(new { Success = false, Message = "Database already installed" });

            //lets save the database settings to config file
            var connectionString = model.ConnectionString;
            var providerName = "SqlServer";

            if (!model.IsConnectionString)
            {
                connectionString = DatabaseManager.CreateConnectionString(model.ServerUrl, model.DatabaseName,
                   model.DatabaseUserName, model.DatabasePassword, false, 0);
            }

            //check if we have correct connection string
            if (!DatabaseManager.DatabaseConnects(connectionString))
            {
                return Response(new { Success = false, Message = "Failed to connect to database" });
            }
            var databaseSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            databaseSettings.WriteSettings(connectionString, DatabaseManager.GetProviderName(providerName));

            //perform the installation
            _installationService.Install();

           //then feed the data
            _installationService.FillRequiredSeedData(model.AdminEmail, model.Password, HttpContext.Current.Request.Url.Host);

            return Response(new { Success = true });
        }

        [Route("install/test-connection")]
        [HttpPost]
        public IHttpActionResult TestConnection(InstallationRequestModel model)
        {
            if (model == null)
                return Response(new {Success = false});

            var connectionString = model.ConnectionString;
            if (!model.IsConnectionString)
            {
                connectionString = DatabaseManager.CreateConnectionString(model.ServerUrl, model.DatabaseName,
                    model.DatabaseUserName, model.DatabasePassword, false, 0);
            }

            var connectionSucceeds = DatabaseManager.DatabaseConnects(connectionString);
            return Response(new { Success = connectionSucceeds });
        }
    }
}