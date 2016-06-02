using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Sponsors
{
    public class SponsorPass : BaseEntity
    {
        public int UserId { get; set; }

        public int SponsorPassOrderId { get; set; }

        public PassStatus Status { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }
    }

    public class SponsorPassMap: BaseEntityConfiguration<SponsorPass> { }
}
