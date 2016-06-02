using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattleParticipant : BaseEntity
    {
        public int VideoBattleId { get; set; }

        public int ParticipantId { get; set; }

        public BattleParticipantStatus ParticipantStatus { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Remarks { get; set; }
        
        public virtual VideoBattle VideoBattle { get; set; }
    }

    public class VideoBattleParticipantMap: BaseEntityConfiguration<VideoBattleParticipant> { }
}
