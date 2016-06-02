using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Sponsors
{
    public class Sponsor : BaseEntity
    {
        public int UserId { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public decimal SponsorshipAmount { get; set; }

        public SponsorshipStatus SponsorshipStatus { get; set; }

        public SponsorshipType SponsorshipType { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }

    public class SponsorMap : BaseEntityConfiguration<Sponsor> { }
}
