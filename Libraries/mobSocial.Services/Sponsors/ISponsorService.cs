using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Sponsors
{
    public interface ISponsorService : IBaseEntityService<Sponsor>
    {
        IList<Sponsor> GetSponsors(int? sponsorCustomerId, int? battleId, BattleType? battleType, SponsorshipStatus? sponsorshipStatus);

        IList<Sponsor> GetSponsorsGrouped(int? sponsorCustomerId, int? battleId, BattleType? battleType, SponsorshipStatus? sponsorshipStatus);
 
        void UpdateSponsorStatus(int sponsorCustomerId, int battleId, BattleType battleType,
            SponsorshipStatus sponsorshipStatus);

        void SaveSponsorData(SponsorData sponsorData);

        SponsorData GetSponsorData(int battleId, BattleType battleType, int sponsorCustomerId);
        IList<SponsorData> GetSponsorData(int battleId, BattleType battleType);

        bool IsSponsor(int sponsorCustomerId, int battleId, BattleType battleType);
    }
}