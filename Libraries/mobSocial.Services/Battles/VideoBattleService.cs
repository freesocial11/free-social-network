using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;
using mobSocial.Services.Emails;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Timeline;
using mobSocial.Services.Users;

namespace mobSocial.Services.Battles
{
    public class VideoBattleService : MobSocialEntityService<VideoBattle>, IVideoBattleService
    {
        private readonly IDataRepository<VideoBattle> _videoBattleRepository;
        private readonly IDataRepository<VideoBattleParticipant> _videoBattleParticipantRepository;
        private readonly IDataRepository<VideoBattleGenre> _videoBattleGenreRepository;
        private readonly IDataRepository<VideoBattleVideo> _videoBattleVideoRepository;

        private readonly IFollowService _customerFollowService;
        private readonly IUserService _customerService;
        private readonly IEmailSender _mobSocialMessageService;
        private readonly ITimelineAutoPublisher _timelineAutoPublisher;

        public VideoBattleService(IDataRepository<VideoBattle> videoBattleRepository,
            IDataRepository<VideoBattleParticipant> videoBattleParticpantRepository,
            IDataRepository<VideoBattleGenre> videoBattleGenreRepository,
            IDataRepository<VideoBattleVideo> videoBattleVideoRepository,
            IMediaService pictureService,
            IFollowService customerFollowService,
            IEmailSender mobSocialMessageService, 
            IUserService customerService, 
            ITimelineAutoPublisher timelineAutoPublisher)
            : base(videoBattleRepository)
        {
            _customerFollowService = customerFollowService;
            _mobSocialMessageService = mobSocialMessageService;
            _customerService = customerService;
            _timelineAutoPublisher = timelineAutoPublisher;
            _videoBattleRepository = videoBattleRepository;
            _videoBattleParticipantRepository = videoBattleParticpantRepository;
            _videoBattleGenreRepository = videoBattleGenreRepository;
            _videoBattleVideoRepository = videoBattleVideoRepository;
        }

        /// <summary>
        /// A multipurpose method for getting the video battles
        /// </summary>
        public IList<VideoBattle> GetAll(int? ownerId, int? participantId, int? videoGenreId, BattleStatus? battleStatus, BattleParticipationType? battleParticipationType, bool? isSponsorshipSupported, string searchTerm, BattlesSortBy? battlesSortBy, SortOrder? sortOrder, out int totalPages, int page = 1, int count = 15)
        {
            var battles = _videoBattleRepository.Get(x => true);
            if (ownerId != null)
            {
                battles = battles.Where(x => x.ChallengerId == ownerId.Value);
            }

            if (participantId != null)
            {
                var participantBattleIds =
                    _videoBattleParticipantRepository.Get(x => x.ParticipantId == participantId.Value)
                        .Select(x => x.VideoBattleId);

                battles = battles.Where(x => participantBattleIds.Contains(x.Id));
            }

            if (videoGenreId != null)
            {
                var videoGenreBattleIds =
                    _videoBattleGenreRepository.Get(x => x.VideoGenreId == videoGenreId.Value)
                        .Select(x => x.VideoBattleId);
                battles = battles.Where(x => videoGenreBattleIds.Contains(x.Id));
            }

            if (battleStatus != null)
            {
                battles = battles.Where(x => x.VideoBattleStatus == battleStatus.Value);
            }

            if (battleParticipationType != null)
            {
                battles = battles.Where(x => x.ParticipationType == battleParticipationType.Value);
            }

            if (isSponsorshipSupported != null)
            {
                battles = battles.Where(x => x.IsSponsorshipSupported);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                battles = battles.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()));
            }
            totalPages = int.Parse(Math.Ceiling((decimal)battles.Count() / count).ToString());

            IOrderedQueryable<VideoBattle> orderedBattles;
            if (battlesSortBy.HasValue)
            {
                switch (battlesSortBy)
                {
                    case BattlesSortBy.PrizeAmount:
                        orderedBattles = sortOrder == SortOrder.Ascending
                            ? battles.OrderBy(x => x.Prizes.Sum(z => z.PrizeAmount))
                            : battles.OrderByDescending(x => x.Prizes.Sum(z => z.PrizeAmount));
                        break;
                    case BattlesSortBy.VotingStartDate:
                        orderedBattles = sortOrder == SortOrder.Ascending
                            ? battles.OrderBy(x => x.VotingStartDate)
                            : battles.OrderByDescending(x => x.VotingStartDate);
                        break;
                    case BattlesSortBy.VotingEndDate:
                        orderedBattles = sortOrder == SortOrder.Ascending
                            ? battles.OrderBy(x => x.VotingEndDate)
                            : battles.OrderByDescending(x => x.VotingEndDate);
                        break;
                    case BattlesSortBy.SponsorshipAmount:
                        orderedBattles = sortOrder == SortOrder.Ascending
                            ? battles.OrderBy(x => x.MinimumSponsorshipAmount)
                            : battles.OrderByDescending(x => x.MinimumSponsorshipAmount);
                        break;
                    case BattlesSortBy.NumberOfParticipants:
                        var tempBattles = battles.Join(_videoBattleParticipantRepository.Get(x => true), b => b.Id, p => p.VideoBattleId,
                            (b, p) => new { Battle = b, ParticipantCount = _videoBattleParticipantRepository.Count(x => x.VideoBattleId == b.Id) });
                        orderedBattles = (sortOrder == SortOrder.Ascending)
                            ? tempBattles.OrderBy(x => x.ParticipantCount).Select(x => x.Battle).OrderBy(x => x.Id)
                            : tempBattles.OrderByDescending(x => x.ParticipantCount)
                                .Select(x => x.Battle)
                                .OrderBy(x => x.Id);


                        break;
                    default:
                        orderedBattles = sortOrder == SortOrder.Ascending
                            ? battles.OrderBy(x => x.Id)
                            : battles.OrderByDescending(x => x.Id);
                        break;
                }
            }
            else
            {
                orderedBattles = sortOrder == SortOrder.Ascending
                          ? battles.OrderBy(x => x.Id)
                          : battles.OrderByDescending(x => x.Id);
            }

            //return paginated result
            return orderedBattles.Skip((page - 1) * count).Take(count).ToList();
        }

        /// <summary>
        /// Sets all the scheduled battles open for public when they reach the end date. It also changes the participant status of the participants who 
        /// have not responded to the challenge 
        /// </summary>
        public void SetScheduledBattlesOpenOrClosed()
        {
            //get the battles which have acceptance date equal to or less than now
            var now = DateTime.UtcNow;
            var videoBattles =
                _videoBattleRepository.Get(
                    x =>
                        (x.VotingStartDate <= now || x.VotingEndDate <= now) &&
                        (x.VideoBattleStatus == BattleStatus.Pending ||
                         x.VideoBattleStatus == BattleStatus.Locked)).ToList();

            foreach (var battle in videoBattles)
            {
                //do we need to open or complete the battle?
                if (battle.VotingEndDate <= now)
                {
                    //lets complete the battle as it's more than voting last date
                    battle.VideoBattleStatus = BattleStatus.Complete;
                }
                else if (battle.VotingStartDate <= now)
                {
                    //get participants of this battle
                    var participants = _videoBattleParticipantRepository.Get(x => x.VideoBattleId == battle.Id);

                    //all the participants who have not accepted will now be changed to denied
                    foreach (var participant in participants.Where(x => x.ParticipantStatus == BattleParticipantStatus.Challenged || x.ParticipantStatus == BattleParticipantStatus.SignedUp).ToList())
                    {
                        participant.ParticipantStatus = participant.ParticipantStatus == BattleParticipantStatus.SignedUp ? BattleParticipantStatus.ChallengeCancelled : BattleParticipantStatus.ChallengeDenied;
                        _videoBattleParticipantRepository.Update(participant);
                    }

                    //let's see if there are enough participants to open the battle (at least two)
                    if (participants.Count(x => x.ParticipantStatus == BattleParticipantStatus.ChallengeAccepted) > 0)
                    {
                        //and do we have at least two videos for competition
                        var battleVideoCount = _videoBattleVideoRepository.Count(x => x.VideoBattleId == battle.Id);
                        //depending on the number of videos, battle can be either open or closed or complete
                        if (battleVideoCount == 0)
                        {
                            battle.VideoBattleStatus = BattleStatus.Closed;
                        }
                        else if (battleVideoCount > 1)
                        {
                            battle.VideoBattleStatus = BattleStatus.Open;
                        }
                        else
                        {
                            //only one video has been uploaded and by default, he should be the winner? (I guess). BTW the battle is complete then.
                            battle.VideoBattleStatus = BattleStatus.Complete;
                        }

                    }
                    else
                    {
                        //so nobody accepted the challenge...too bad...let's close the battle then
                        battle.VideoBattleStatus = BattleStatus.Closed;
                    }
                }

                _videoBattleRepository.Update(battle);

                //do we need to post to timeline?
                if (battle.AutomaticallyPostEventsToTimeline)
                {
                    switch (battle.VideoBattleStatus)
                    {
                        case BattleStatus.Open:
                            _timelineAutoPublisher.Publish(battle, TimelineAutoPostTypeNames.VideoBattle.BattleStart, battle.ChallengerId);
                            break;
                        case BattleStatus.Complete:
                            _timelineAutoPublisher.Publish(battle, TimelineAutoPostTypeNames.VideoBattle.BattleComplete, battle.ChallengerId);
                            break;
                    }
                }
                //depending on battle status, let's send notifications
                if (battle.VideoBattleStatus == BattleStatus.Open || battle.VideoBattleStatus == BattleStatus.Complete)
                {
                    //get the followers first
                    var followers = _customerFollowService.GetFollowers<VideoBattle>(battle.Id);
                    var followerCustomerIds = followers.Select(x => x.UserId).ToArray();

                    //and their customers
                    var customers = _customerService.Get(x => followerCustomerIds.Contains(x.Id), null);
                    switch (battle.VideoBattleStatus)
                    {
                        case BattleStatus.Open:
                            //send battle open notifications
                            foreach (var customer in customers)
                            {
                                _mobSocialMessageService.SendVideoBattleOpenNotification(customer, battle);
                            }
                            break;
                        case BattleStatus.Complete:
                            //send battle complete notifications
                            foreach (var customer in customers)
                            {
                                _mobSocialMessageService.SendVideoBattleCompleteNotification(customer, battle,
                                    NotificationRecipientType.Voter);
                            }
                            break;
                    }

                }
            }

        }
    }
}