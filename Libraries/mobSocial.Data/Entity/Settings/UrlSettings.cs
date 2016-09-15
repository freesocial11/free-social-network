using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class UrlSettings : ISettingGroup
    {
        /// <summary>
        /// The activation page url that's included in the emails sent for user activation
        /// </summary>
        public string ActivationPageUrl { get; set; }
    }
}
