using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Battles
{
    public class VideoBattleParticipantService : MobSocialEntityService<VideoBattleParticipant>, IVideoBattleParticipantService
    {
        private readonly IDataRepository<VideoBattleParticipant> _videoBattleParticipantRepository;

        public VideoBattleParticipantService(IDataRepository<VideoBattleParticipant> videoBattleParticipantRepository) :
            base(videoBattleParticipantRepository)
        {
            _videoBattleParticipantRepository = videoBattleParticipantRepository;
        }       
      
        public BattleParticipantStatus GetParticipationStatus(int battleId, int participantId)
        {
            var videoBattleParticipant = GetVideoBattleParticipant(battleId, participantId);
            if (videoBattleParticipant == null)
                return BattleParticipantStatus.NotChallenged;

            return videoBattleParticipant.ParticipantStatus;
        }

        public VideoBattleParticipant GetVideoBattleParticipant(int battleId, int participantId)
        {
            var videoBattleParticipant = _videoBattleParticipantRepository.Get(x => x.ParticipantId == participantId && x.VideoBattleId == battleId).FirstOrDefault();
            return videoBattleParticipant;
        }


        public IList<VideoBattleParticipant> GetVideoBattleParticipants(int battleId, BattleParticipantStatus? participantStatus)
        {
            if (participantStatus.HasValue)
            {
                return _videoBattleParticipantRepository.Get(x => x.VideoBattleId == battleId && x.ParticipantStatus == participantStatus.Value).OrderBy(x => x.ParticipantStatus).ToList();
            }
            return _videoBattleParticipantRepository.Get(x => x.VideoBattleId == battleId).OrderBy(x => x.ParticipantStatus).ToList();
        }
        
    }
}
