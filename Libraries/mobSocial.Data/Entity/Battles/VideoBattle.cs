using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattle : BaseEntity, IPermalinkSupported, IHasEntityProperties<VideoBattle>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ChallengerId { get; set; }

        public DateTime VotingStartDate { get; set; }

        public DateTime VotingEndDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public BattleType VideoBattleType { get; set; }

        public BattleStatus VideoBattleStatus { get; set; }

        public BattleVoteType VideoBattleVoteType { get; set; }

        public BattleParticipationType ParticipationType { get; set; }

        public int MaximumParticipantCount { get; set; }

        public bool IsVotingPayable { get; set; }

        public decimal MinimumVotingCharge { get; set; }

        public bool CanVoterIncreaseVotingCharge { get; set; }

        public decimal ParticipantPercentagePerVote { get; set; }

        public IList<VideoBattlePrize> Prizes { get; set; }

        public bool IsSponsorshipSupported { get; set; }

        public decimal MinimumSponsorshipAmount { get; set; }

        public SponsoredCashDistributionType SponsoredCashDistributionType { get; set; }

        public bool AutomaticallyPostEventsToTimeline { get; set; }
    }

    public class VideoBattleMap : BaseEntityConfiguration<VideoBattle>
    {
        public VideoBattleMap()
        {
            
        }
    }
}
