using System.Collections.Generic;
using System.IO;
using System.Web;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services.Configuration;
using mobSocial.Data.Database;

namespace mobSocial.WebApi.Configuration.Database
{
    public class DatabaseSettings : IDatabaseSettings
    {
        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                return _connectionString;
                
            }
        }

        private string _providerName;
        public string ProviderName
        {
            get
            {
                return _providerName;
            }
        }

        public DatabaseSettings()
        {
            LoadSettings();
        }

        private readonly string _saveFileName = HttpContext.Current.Server.MapPath("~/App_Data/database.config");
        public void LoadSettings()
        {
            
            if (!File.Exists(_saveFileName))
                return;

            var configFileService = mobSocialEngine.ActiveEngine.Resolve<IConfigurationFileService>();

            var configValues = configFileService.ReadFile(_saveFileName);
            if(configValues == null)
                throw new mobSocialException("Invalid configuration file");

            _connectionString = configValues["ConnectionString"];
            _providerName = configValues["ProviderName"];

            
        }
        
        public void WriteSettings(string connectionString, string providerName)
        {
            var configFileService = mobSocialEngine.ActiveEngine.Resolve<IConfigurationFileService>();
            var configValues = new Dictionary<string, string>()
            {
                {"ConnectionString", connectionString},
                {"ProviderName", providerName}
            };
            //write settings
            configFileService.WriteFile(_saveFileName, configValues);

            _connectionString = connectionString;
            _providerName = providerName;
        }
    }
}