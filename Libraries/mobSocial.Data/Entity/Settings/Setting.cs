using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Settings
{
    public class Setting : BaseEntity
    {
        public string GroupName { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class SettingMap : BaseEntityConfiguration<Setting> { }
}