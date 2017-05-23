using System.Web.Configuration;

namespace mobSocial.Core
{
    public class ApplicationHelper
    {
        private static void SetConfigurationValue(string name, string value)
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            if (configuration.AppSettings.Settings[name] != null)
                configuration.AppSettings.Settings[name].Value = value;
            else
                configuration.AppSettings.Settings.Add(name, value);
            configuration.Save();
        }

        private static string GetConfigurationValue(string name)
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            return configuration.AppSettings.Settings[name]?.Value;
        }
        public static bool AreTablesInstalled()
        {
            var appData = ServerHelper.GetLocalPathFromRelativePath("~/App_Data/mobSocial");
            return System.IO.File.Exists(appData);
        }

        public static void MarkTablesInstalled(bool installed = true)
        {
            var appData = ServerHelper.GetLocalPathFromRelativePath("~/App_Data/mobSocial");
            System.IO.File.Create(appData);
        }

        public static void SetOwinStartup(bool set)
        {
            var settingName = "owin:AutomaticAppStartup";
            SetConfigurationValue(settingName, set ? "true" : "false");
        }


    }
}