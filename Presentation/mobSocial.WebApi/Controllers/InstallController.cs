using System.Web.Http;
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
            var areTableInstalled = DatabaseManager.IsDatabaseInstalled();

            if (areTableInstalled)
                return Response(new { Success = false, Message = "Database already installed" });

            //lets save the database settings to config file
            var connectionString = "";
            var providerName = "";
            connectionString = @"Data Source=.\sqlexpress;Initial Catalog=roasteddesk;Integrated Security=False;Persist Security Info=False;User ID=iis_user;Password=iis_user";
            providerName = "SqlServer";

            var databaseSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            databaseSettings.WriteSettings(connectionString, providerName);

            //perform the installation
            _installationService.Install();

           //then feed the data
            _installationService.FillRequiredSeedData(model.AdminEmail, model.Password);

            return Response(new { Success = true });
        }
    }
}