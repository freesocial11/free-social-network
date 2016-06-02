using System;
using System.Collections.Generic;
using AutoMapper;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Models.Battles;

namespace mobSocial.Tests.SampleData
{
    public static class SampleVideoBattles
    {
        private static readonly List<VideoBattle> SampleBattles;

        static SampleVideoBattles()
        {
            SampleBattles = new List<VideoBattle>();
        }

        public static List<VideoBattle> GetSampleVideoBattles()
        {
            return SampleBattles;   
        }

        public static VideoBattle GetSampleVideoBattle(int index)
        {
            return SampleBattles[index];
        }

        public static List<VideoBattleModel> GetSampleVideoBattleModels()
        {
            Mapper.Initialize(x => x.CreateMap<VideoBattle, VideoBattleModel>());
            var list = new List<VideoBattleModel>();
            Mapper.Map(SampleBattles, list);
            return list;
        }

        public static VideoBattleModel GetSampleVideoBattleModel(int index)
        {
            Mapper.Initialize(x => x.CreateMap<VideoBattle, VideoBattleModel>());
            var list = new List<VideoBattleModel>();
            Mapper.Map(SampleBattles, list);
            return list[index];
        }

        static void PopulateVideoBattles()
        {
            SampleBattles.Add(new VideoBattle() {
                Name = "First Battle Is Awesome",
                AutomaticallyPostEventsToTimeline = true,
                IsVotingPayable = true,
                CanVoterIncreaseVotingCharge = true,
                IsSponsorshipSupported = true,
                ParticipationType = BattleParticipationType.Open,
                MinimumSponsorshipAmount = 1,
                VideoBattleVoteType = BattleVoteType.SelectOneWinner,
                MinimumVotingCharge = 1,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                VotingStartDate = DateTime.UtcNow.AddDays(25),
                VotingEndDate = DateTime.UtcNow.AddDays(45)
            });

            SampleBattles.Add(new VideoBattle() {
                Name = "Second is Cool Battle",
                AutomaticallyPostEventsToTimeline = false,
                IsVotingPayable = true,
                CanVoterIncreaseVotingCharge = true,
                IsSponsorshipSupported = true,
                ParticipationType = BattleParticipationType.InviteOnly,
                MinimumSponsorshipAmount = 5,
                VideoBattleVoteType = BattleVoteType.LikeDislike,
                MinimumVotingCharge = 5,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                VotingStartDate = DateTime.UtcNow.AddDays(15),
                VotingEndDate = DateTime.UtcNow.AddDays(65)
            });

            SampleBattles.Add(new VideoBattle() {
                Name = "Third battle is super",
                AutomaticallyPostEventsToTimeline = true,
                IsVotingPayable = true,
                CanVoterIncreaseVotingCharge = true,
                IsSponsorshipSupported = true,
                ParticipationType = BattleParticipationType.Open,
                MaximumParticipantCount = 5,
                VideoBattleVoteType = BattleVoteType.SelectOneWinner,
                MinimumVotingCharge = 1,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                VotingStartDate = DateTime.UtcNow.AddDays(25),
                VotingEndDate = DateTime.UtcNow.AddDays(45)
            });

            SampleBattles.Add(new VideoBattle() {
                Name = "fourth battle is small cased",
                AutomaticallyPostEventsToTimeline = true,
                IsVotingPayable = true,
                CanVoterIncreaseVotingCharge = true,
                IsSponsorshipSupported = true,
                ParticipationType = BattleParticipationType.Open,
                MinimumSponsorshipAmount = 1,
                VideoBattleVoteType = BattleVoteType.Rating,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow.AddHours(5),
                VotingStartDate = DateTime.UtcNow.AddDays(25),
                VotingEndDate = DateTime.UtcNow.AddDays(45)
            });
        }
    }
}
