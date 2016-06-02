using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoBattleModel : RootEntityModel
    {
        public VideoBattleModel()
        {
            Prizes = new List<VideoBattlePrizeModel>();
        }

        [Required]
        public string Name { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [Required]
        public int ChallengerId { get; set; }

        public DateTime VotingStartDate { get; set; }

        public DateTime VotingEndDate { get; set; }

        public BattleParticipationType VideoBattleParticipationType { get; set; }

        public BattleStatus VideoBattleStatus { get; set; }

        public BattleVoteType VideoBattleVoteType { get; set; }

        public int MaximumParticipantCount { get; set; }

        public bool IsVotingPayable { get; set; }

        public decimal MinimumVotingCharge { get; set; }

        public bool CanVoterIncreaseVotingCharge { get; set; }

        public decimal ParticipantPercentagePerVote { get; set; }

        public IList<VideoBattlePrizeModel> Prizes { get; set; }

        public bool IsSponsorshipSupported { get; set; }

        public decimal MinimumSponsorshipAmount { get; set; }

        public SponsoredCashDistributionType SponsoredCashDistributionType { get; set; }

        public bool AutomaticallyPostEventsToTimeline { get; set; }
    }
}
