using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class SkillSettings : ISettingGroup
    {
        public int NumberOfUsersPerPageOnSinglePage { get; set; }
    }
}