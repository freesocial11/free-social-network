using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class SystemSettings : ISettingGroup
    {
        /// <summary>
        /// Is the application installed?
        /// </summary>
        public bool IsInstalled { get; set; }
    }
}