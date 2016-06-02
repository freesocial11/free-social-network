using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Sponsors
{
    public class SponsorService : MobSocialEntityService<Sponsor>, ISponsorService
    {
        private readonly IDataRepository<SponsorData> _sponsorDataRepository;

        public SponsorService(IDataRepository<Sponsor> repository,
            IDataRepository<SponsorData> sponsorDataRepository)
            : base(repository)
        {
            _sponsorDataRepository = sponsorDataRepository;
        }
       
        public IList<Sponsor> GetSponsors(int? sponsorCustomerId, int? battleId, BattleType? battleType, SponsorshipStatus? sponsorshipStatus)
        {
            var query = Repository.Get(x => true);

            if (sponsorCustomerId.HasValue)
                query = query.Where(x => x.UserId == sponsorCustomerId);

            if (battleId.HasValue)
                query = query.Where(x => x.BattleId == battleId);

            if (battleType.HasValue)
                query = query.Where(x => x.BattleType == battleType);

            if (sponsorshipStatus.HasValue)
                query = query.Where(x => x.SponsorshipStatus == sponsorshipStatus);

            return query.ToList();
        }

        /// <summary>
        /// Groups the list of sponsors so that sponsors with multiple trasactions get summed up into a single object
        /// </summary>
        /// <param name="sponsorUserId"></param>
        /// <param name="battleId"></param>
        /// <param name="battleType"></param>
        /// <returns></returns>
        public IList<Sponsor> GetSponsorsGrouped(int? sponsorUserId, int? battleId, BattleType? battleType, SponsorshipStatus? sponsorshipStatus)
        {
            var sponsors = GetSponsors(sponsorUserId, battleId, battleType, sponsorshipStatus);
            if (sponsors.Any() && sponsors.Count > 1)
            {
                sponsors = sponsors.GroupBy(x => new { x.UserId, x.BattleId, x.BattleType, x.SponsorshipStatus }).Select(g => new Sponsor() {
                    BattleId = g.Key.BattleId,
                    BattleType = g.Key.BattleType,
                    UserId = g.Key.UserId,
                    SponsorshipStatus = g.Key.SponsorshipStatus,
                    SponsorshipAmount = g.Sum(x => x.SponsorshipAmount)

                }).ToList();
            }

            return sponsors;
        }

        public void UpdateSponsorStatus(int sponsorUserId, int battleId, BattleType battleType, SponsorshipStatus sponsorshipStatus)
        {
            var sponsors = GetSponsors(sponsorUserId, battleId, battleType, null);
            foreach (var sponsor in sponsors)
            {
                sponsor.SponsorshipStatus = sponsorshipStatus;
                //update
                Update(sponsor);
                //TODO: Update all in one go to improve performance
            }

            //if it's an approval, lets save sponsor data which will contain details about the sponsorship display part
            if (sponsorshipStatus != SponsorshipStatus.Accepted)
                return;

            var sponsorData = GetSponsorData(battleId, battleType, sponsorUserId) ?? new SponsorData() {
                BattleType = battleType,
                BattleId = battleId,
                SponsorCustomerId = sponsorUserId,
                PictureId = 0,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                DisplayName = "",
                DisplayOrder = 0,
                TargetUrl = ""
            };

            SaveSponsorData(sponsorData);
        }


        public void SaveSponsorData(SponsorData sponsorData)
        {
            if (sponsorData.Id == 0)
                _sponsorDataRepository.Insert(sponsorData);
            else
                _sponsorDataRepository.Update(sponsorData);

        }

        public SponsorData GetSponsorData(int battleId, BattleType battleType, int sponsorCustomerId)
        {
            var query = _sponsorDataRepository.Get(x => x.BattleType == battleType && x.BattleId == battleId && x.SponsorCustomerId == sponsorCustomerId);

            return query.FirstOrDefault();
        }

        public IList<SponsorData> GetSponsorData(int battleId, BattleType battleType)
        {
            return _sponsorDataRepository.Get(x => x.BattleType == battleType && x.BattleId == battleId).ToList();
        }

        public bool IsSponsor(int sponsorUserId, int battleId, BattleType battleType)
        {
            return
                Repository.Get(
                    x => x.BattleId == battleId && x.BattleType == battleType && x.UserId == sponsorUserId)
                    .Any();
        }
    }
}