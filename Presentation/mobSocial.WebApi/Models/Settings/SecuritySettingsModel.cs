using System.Collections.Generic;
using mobSocial.Data.Enum;

namespace mobSocial.WebApi.Models.Settings
{
    public class SecuritySettingsModel
    {
        public SecuritySettingsModel()
        {
            AvailablePasswordStorageFormats = new List<dynamic>();
        }
        /// <summary>
        /// Default password format
        /// </summary>
        public PasswordFormat DefaultPasswordStorageFormat { get; set; }

        public List<dynamic> AvailablePasswordStorageFormats { get; set; }
    }
}