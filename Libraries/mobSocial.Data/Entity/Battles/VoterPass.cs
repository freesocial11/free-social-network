using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Battles
{
    public class VoterPass : BaseEntity
    {
        public int CustomerId { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public int VoterPassOrderId { get; set; }

        public PassStatus Status { get; set; }
    }

    public class VoterPassMap: BaseEntityConfiguration<VoterPass> { }
}
