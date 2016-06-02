using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class DateTimeSettings : ISettingGroup
    {
        public string DefaultTimeZoneId { get; set; }
    }
}