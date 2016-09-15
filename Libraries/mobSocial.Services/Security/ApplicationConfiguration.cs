using System.Web.Configuration;

namespace mobSocial.Services.Security
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string GetSetting(string settingName)
        {
            return WebConfigurationManager.AppSettings[settingName];
        }

        public void SetSetting(string settingName, string value)
        {
            //open the configuration
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            config.AppSettings.Settings[settingName].Value = value;
            config.Save();
        }
    }
}
