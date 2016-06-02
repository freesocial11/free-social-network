using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattleVote: BaseEntity
    {
        public int UserId { get; set; }

        public int VideoBattleId { get; set; }

        public int ParticipantId { get; set; }

        public int VoteValue { get; set; }

        public BattleVoteStatus VoteStatus { get; set; }

        public virtual VideoBattle VideoBattle { get; set; }
    }

    public class VideoBattleVoteMap: BaseEntityConfiguration<VideoBattleVote> { }
}
