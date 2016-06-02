using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Sponsors
{
    public class SponsorData : BaseEntity
    {
        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public int SponsorCustomerId { get; set; }

        public int PictureId { get; set; }

        public int DisplayOrder { get; set; }

        public string TargetUrl { get; set; }

        public string DisplayName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }

    public class SponsorDataMap: BaseEntityConfiguration<SponsorData> { }
}
