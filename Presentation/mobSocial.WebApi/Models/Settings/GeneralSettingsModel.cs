using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mobSocial.WebApi.Models.Settings
{
    public class GeneralSettingsModel
    {
        public GeneralSettingsModel()
        {
            AvailableTimeZones = new List<dynamic>();    
        }

        /// <summary>
        /// The domain where public website has been installed
        /// </summary>
        [Required]
        public string ApplicationUiDomain { get; set; }

        /// <summary>
        /// The domain where api has been installed
        /// </summary>
        [Required]
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
        [Required]
        public string DefaultTimeZoneId { get; set; }

        /// <summary>
        /// Gets available timezones on the server
        /// </summary>
        public List<dynamic> AvailableTimeZones { get; set; }
    }
}