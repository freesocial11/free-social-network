using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class GeneralSettings : ISettingGroup
    {
        /// <summary>
        /// The domain where public website has been installed
        /// </summary>
        public string ApplicationUiDomain { get; set; }

        /// <summary>
        /// The domain where api has been installed
        /// </summary>
        public string ApplicationApiRoot { get; set; }

        /// <summary>
        /// The domain for which authentication cookie is issued. Keep this to a cross domain value (that begins with a .) for example .mobsocial.co
        /// </summary>
        public string ApplicationCookieDomain { get; set; }

        /// <summary>
        /// The domain where images are served. Default is same as ApplicationApiRoot
        /// </summary>
        public string ImageServerDomain { get; set; }

        /// <summary>
        /// The domain where videos are served. Default is same as ApplicationApiRoot
        /// </summary>
        public string VideoServerDomain { get; set; }

        /// <summary>
        /// Default timezone to be used for network
        /// </summary>
        public string DefaultTimeZoneId { get; set; }

    }
}