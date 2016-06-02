using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Core;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Credits;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Videos;
using mobSocial.Data.Enum;
using mobSocial.Services.Battles;
using mobSocial.Services.Credits;
using mobSocial.Services.Emails;
using mobSocial.Services.Extensions;
using mobSocial.Services.Formatter;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Payments;
using mobSocial.Services.Settings;
using mobSocial.Services.Social;
using mobSocial.Services.Sponsors;
using mobSocial.Services.Timeline;
using mobSocial.Services.Users;
using mobSocial.Services.Videos;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions;
using mobSocial.WebApi.Models.Battles;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("videobattles")]
    public class VideoBattleController : RootApiController
    {
        private readonly IVideoBattleService _videoBattleService;
        private readonly IVideoBattleParticipantService _videoBattleParticipantService;
        private readonly IVideoBattleVideoService _videoBattleVideoService;
        private readonly IVideoBattleVoteService _videoBattleVoteService;
        private readonly IVideoBattlePrizeService _videoBattlePrizeService;
        private readonly IUserService _userService;
        private readonly IWatchedVideoService _watchedVideoService;
        private readonly IMediaService _pictureService;
        private readonly ISponsorService _sponsorService;
        private readonly ITimelineAutoPublisher _timelineAutoPublisher;
        private readonly ISettingService _settingService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly IFollowService _followService;
        private readonly ICreditService _creditService;
        private readonly IFormatterService _formatterService;
        private readonly IEmailSender _emailSender;
        private readonly IMobSocialVideoProcessor _videoProcessor;
        private readonly MediaSettings _mediaSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly BattleSettings _battleSettings;
        private readonly GeneralSettings _generalSettings;

        #region ctor

        public VideoBattleController(
            IVideoBattleService videoBattleService,
            IVideoBattleParticipantService videoBattleParticipantService,
            IVideoBattleVideoService videoBattleVideoService,
            IVideoBattleVoteService videoBattleVoteService,
            IVideoBattlePrizeService videoBattlePrizeService,
            IUserService userService,
            IWatchedVideoService watchedVideoService,
            IMediaService pictureService,
            ISponsorService sponsorService,
            ITimelineAutoPublisher timelineAutoPublisher,
            ISettingService settingService,
            IPaymentProcessingService paymentProcessingService,
            IFollowService followService,
            ICreditService creditService,
            IFormatterService formatterService,
            IEmailSender emailSender,
            IMobSocialVideoProcessor videoProcessor,
            MediaSettings mediaSettings,
            PaymentSettings paymentSettings,
            BattleSettings battleSettings,
            GeneralSettings generalSettings)
        {
            _videoBattleService = videoBattleService;
            _videoBattleParticipantService = videoBattleParticipantService;
            _videoBattleVideoService = videoBattleVideoService;
            _videoBattleVoteService = videoBattleVoteService;
            _videoBattlePrizeService = videoBattlePrizeService;
            _userService = userService;
            _watchedVideoService = watchedVideoService;
            _timelineAutoPublisher = timelineAutoPublisher;
            _settingService = settingService;
            _paymentProcessingService = paymentProcessingService;
            _followService = followService;
            _creditService = creditService;
            _paymentSettings = paymentSettings;
            _battleSettings = battleSettings;
            _formatterService = formatterService;
            _emailSender = emailSender;
            _generalSettings = generalSettings;
            _videoProcessor = videoProcessor;
            _sponsorService = sponsorService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Battles

        [Authorize]
        [Route("edit/{videoBattleId:int}")]
        [HttpGet]
        public IHttpActionResult Edit(int videoBattleId = 0)
        {
            var videoBattle = videoBattleId != 0 ? _videoBattleService.Get(videoBattleId) : new VideoBattle();

            //can the user actually edit the battle?
            if (!CanEdit(videoBattle))
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "edit_battle");
                return RespondFailure();
            }

            var model = new VideoBattleModel() {
                VotingStartDate =
                    videoBattleId == 0 ? DateTime.UtcNow.AddDays(10) : videoBattle.VotingStartDate.ToLocalTime(),
                //10 days
                ChallengerId = videoBattle.ChallengerId,
                DateCreated = videoBattleId == 0 ? DateTime.UtcNow : videoBattle.DateCreated,
                DateUpdated = videoBattleId == 0 ? DateTime.UtcNow : videoBattle.DateUpdated,
                VotingEndDate =
                    videoBattleId == 0 ? DateTime.UtcNow.AddDays(20) : videoBattle.VotingEndDate.ToLocalTime(),
                //20 days
                Description = videoBattle.Description,
                Name = videoBattle.Name,
                Id = videoBattle.Id,
                VideoBattleStatus = videoBattle.VideoBattleStatus,
                VideoBattleParticipationType = videoBattleId == 0 ? BattleParticipationType.Open : videoBattle.ParticipationType,
                VideoBattleVoteType = videoBattle.VideoBattleVoteType,
                MaximumParticipantCount = videoBattleId == 0 ? 10 : videoBattle.MaximumParticipantCount,
                MinimumVotingCharge =
                    videoBattleId == 0
                        ? _battleSettings.DefaultVotingChargeForPaidVoting
                        : videoBattle.MinimumVotingCharge,
                IsVotingPayable = videoBattle.IsVotingPayable,
                CanVoterIncreaseVotingCharge = videoBattle.CanVoterIncreaseVotingCharge,
                ParticipantPercentagePerVote = videoBattle.ParticipantPercentagePerVote,
                IsSponsorshipSupported = videoBattle.IsSponsorshipSupported,
                MinimumSponsorshipAmount = videoBattle.MinimumSponsorshipAmount,
                SponsoredCashDistributionType = videoBattle.SponsoredCashDistributionType,
                AutomaticallyPostEventsToTimeline = videoBattle.AutomaticallyPostEventsToTimeline
            };


            //let's get prizes associated with this battle. prizes can only be added to saved battles
            if (model.Id != 0)
            {
                var prizes = _videoBattlePrizeService.GetBattlePrizes(model.Id).Where(x => !x.IsSponsored);
                foreach (var prize in prizes)
                {
                    model.Prizes.Add(new VideoBattlePrizeModel() {
                        Id = prize.Id,
                        VideoBattleId = prize.VideoBattleId,
                        PrizeType = prize.PrizeType,
                        WinnerId = prize.WinnerId,
                        PrizeAmount = prize.PrizeAmount,
                        Description = prize.Description,
                        PrizeOther = prize.PrizeOther,
                        PrizePercentage = prize.PrizePercentage,
                        WinnerPosition = prize.WinnerPosition,
                        PrizeProductId = prize.PrizeProductId
                    });
                }
            }
            return RespondSuccess(model);
        }

        [HttpPost]
        [Authorize]
        [Route("savebattle")]
        public IHttpActionResult SaveVideoBattle(VideoBattleModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                VerboseReporter.ReportError("Missing required parameters or invalid data submitted", "save_video_battle");
                return RespondFailure();
            }

            //lets check if it's a new video battle or an edit is being performed
            VideoBattle videoBattle = null;
            if (model.Id == 0)
            {
                videoBattle = new VideoBattle {
                    ChallengerId = ApplicationContext.Current.CurrentUser.Id,
                    DateCreated = DateTime.UtcNow
                };
            }
            else
            {
                videoBattle = _videoBattleService.Get(model.Id);
            }


            videoBattle.DateUpdated = DateTime.UtcNow;
            videoBattle.Description = model.Description;
            videoBattle.VideoBattleStatus = BattleStatus.Pending;
            videoBattle.VideoBattleType = BattleType.Video;
            videoBattle.VideoBattleVoteType = BattleVoteType.SelectOneWinner; // Model.VideoBattleVoteType;
            videoBattle.Name = model.Name;
            videoBattle.MaximumParticipantCount = (int)Math.Round((decimal)model.MaximumParticipantCount);
            videoBattle.IsVotingPayable = model.MinimumVotingCharge > 0 && model.IsVotingPayable;
            videoBattle.CanVoterIncreaseVotingCharge = model.CanVoterIncreaseVotingCharge;
            videoBattle.MinimumVotingCharge = model.MinimumVotingCharge;
            videoBattle.ParticipantPercentagePerVote = model.ParticipantPercentagePerVote;
            videoBattle.IsSponsorshipSupported = model.IsSponsorshipSupported;
            videoBattle.MinimumSponsorshipAmount = model.MinimumSponsorshipAmount;
            videoBattle.SponsoredCashDistributionType = model.SponsoredCashDistributionType;
            videoBattle.AutomaticallyPostEventsToTimeline = model.AutomaticallyPostEventsToTimeline;
            if (model.Id == 0)
            {
                videoBattle.VotingStartDate = model.VotingStartDate.ToUniversalTime();
                videoBattle.VotingEndDate = model.VotingEndDate.ToUniversalTime();
                _videoBattleService.Insert(videoBattle);

                //post to timeline if required
                if (model.AutomaticallyPostEventsToTimeline)
                    _timelineAutoPublisher.Publish(videoBattle, TimelineAutoPostTypeNames.VideoBattle.Publish,
                        ApplicationContext.Current.CurrentUser.Id);
            }
            else
            {
                //its an update...if there is any participant who has accepted the challenge then acceptance date and voting date can only be extended and not shrinked
                if (model.VotingStartDate.ToUniversalTime() < videoBattle.VotingStartDate ||
                    model.VotingEndDate.ToUniversalTime() < videoBattle.VotingEndDate)
                {
                    //so this is the case. lets see if we have any participants who have accepted the challenge
                    var participants = _videoBattleParticipantService.GetVideoBattleParticipants(model.Id,
                        BattleParticipantStatus.ChallengeAccepted);
                    if (participants.Count > 0)
                    {
                        //nop, somebody has accepted the challenge. date can only be exended
                        VerboseReporter.ReportError("Acceptance and Voting dates can only be extended now, because a participant has accepted the challenge", "save_video_battle");
                        return RespondFailure();
                    }
                }
                videoBattle.VotingStartDate = model.VotingStartDate.ToUniversalTime();
                videoBattle.VotingEndDate = model.VotingEndDate.ToUniversalTime();
                if (CanEdit(videoBattle))
                    _videoBattleService.Update(videoBattle);
                else
                {
                    VerboseReporter.ReportError("Unauthorized", "save_video_battle");
                    return RespondFailure();
                }
            }
            return RespondSuccess(new {
                Id = videoBattle.Id,
                SeName = videoBattle.GetPermalink()
            });
        }

        [Authorize]
        [HttpPost]
        [Route("saveprize")]
        public IHttpActionResult SavePrize(VideoBattlePrizeModel model)
        {
            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Missing required parameters or invalid data submitted", "save_prize");
                return RespondFailure();
            }

            //does the person adding or updating prize own this battle?
            var videoBattle = _videoBattleService.Get(model.VideoBattleId);
            if (videoBattle == null || videoBattle.ChallengerId != ApplicationContext.Current.CurrentUser.Id)
            {
                VerboseReporter.ReportError("Unauthorized", "save_prize");
                return RespondFailure();
            }

            VideoBattlePrize prize = null;
            //let's check if the prize is being edited or added
            if (model.Id == 0)
            {
                prize = new VideoBattlePrize() {
                    DateCreated = DateTime.UtcNow
                };
            }
            else
            {
                prize = _videoBattlePrizeService.Get(model.Id);
            }
            prize.DateUpdated = DateTime.UtcNow;
            prize.Description = model.Description;
            switch (model.PrizeType)
            {
                case BattlePrizeType.FixedAmount:
                    prize.PrizeAmount = model.PrizeAmount;
                    break;
                case BattlePrizeType.FixedProduct:
                    prize.PrizeProductId = model.PrizeProductId;
                    break;
                case BattlePrizeType.PercentageAmount:
                    prize.PrizePercentage = model.PrizePercentage;
                    break;
                case BattlePrizeType.Other:
                    prize.PrizeOther = model.PrizeOther;
                    break;
            }

            prize.WinnerId = model.WinnerId;
            prize.WinnerPosition = model.WinnerPosition;
            prize.PrizeType = model.PrizeType;
            prize.VideoBattleId = model.VideoBattleId;

            if (prize.Id == 0)
                _videoBattlePrizeService.Insert(prize);
            else
                _videoBattlePrizeService.Update(prize);

            return RespondSuccess(new { Id = prize.Id });
        }

        [HttpDelete]
        [Authorize]
        [Route("deleteprize/{videoBattleId:int}/{prizeId:int}")]
        public IHttpActionResult DeletePrize(int videoBattleId, int prizeId)
        {
            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "delete_prize");
                return RespondFailure();
            }
            //does the person adding or updating prize own this battle?
            var videoBattle = _videoBattleService.Get(videoBattleId);
            if (videoBattle == null || videoBattle.ChallengerId != ApplicationContext.Current.CurrentUser.Id)
            {
                VerboseReporter.ReportError("Unauthorized", "delete_prize");
                return RespondFailure();
            }

            var prize = _videoBattlePrizeService.Get(prizeId);
            if (prize == null)
            {
                VerboseReporter.ReportError("Prize doesn't exist", "delete_prize");
                return RespondFailure();
            }
            _videoBattlePrizeService.Delete(prize);
            return RespondSuccess(new { Id = prize.Id });
        }

        [HttpDelete]
        [Authorize]
        [Route("deletevideobattle/{videoBattleId:int}")]
        public IHttpActionResult DeleteVideoBattle(int videoBattleId)
        {
            var videoBattle = _videoBattleService.Get(videoBattleId);

            //does the video battle exist?
            if (videoBattle == null)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "delete_video_battle");
                return RespondFailure();
            }

            //only the person who is logged must be the owner of battle or admin
            if (ApplicationContext.Current.CurrentUser.IsAdministrator() ||
                ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId)
            {
                //we should delete all the participants, videos, votes, etc. before deleting battle

                //videos
                var videos = _videoBattleVideoService.GetBattleVideos(videoBattleId);
                foreach (var video in videos)
                {
                    //delete video file from server
                    var path = HostingEnvironment.MapPath(video.VideoPath);
                    if (!string.IsNullOrEmpty(path))
                        File.Delete(path);

                    _videoBattleVideoService.Delete(video);
                }

                //votes
                var votes = _videoBattleVoteService.GetVideoBattleVotes(videoBattleId, null);
                foreach (var vote in votes)
                {
                    _videoBattleVoteService.Delete(vote);
                }
                //participants
                var participants = _videoBattleParticipantService.GetVideoBattleParticipants(videoBattleId, null);
                foreach (var participant in participants)
                {
                    _videoBattleParticipantService.Delete(participant);
                }
            }
            //now delete video battle
            _videoBattleService.Delete(videoBattle);
            return RespondSuccess();
        }

        [HttpGet]
        [Route("get/{id}/{viewMode}")]
        public IHttpActionResult Get(int id, VideoViewMode viewMode = VideoViewMode.Regular)
        {
            //let's get the video battle by it's slug
            var videoBattle = _videoBattleService.Get(id);

            //does the video battle exist?
            if (videoBattle == null)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "get_battle");
                return RespondFailure();
            }

            var videoBattleId = videoBattle.Id;
            IList<VideoBattleParticipant> participants = null;

            //it's quite possible that battle is about to open/close and we are waiting for scheduler to open/close the battle...we should lock it then
            //so that nobody can do anything with it now.
            if ((videoBattle.VotingStartDate <= DateTime.UtcNow && videoBattle.VideoBattleStatus == BattleStatus.Pending) ||
                (videoBattle.VotingEndDate <= DateTime.UtcNow && videoBattle.VideoBattleStatus == BattleStatus.Open))
            {
                videoBattle.VideoBattleStatus = BattleStatus.Locked;
                _videoBattleService.Update(videoBattle);
            }

            //only open video battles can be viewed or ofcourse if I am the owner, I should be able to see it open or closed right?
            var canOpen = videoBattle.VideoBattleStatus != BattleStatus.Pending
                          || CanEdit(videoBattle)
                          || (videoBattle.ParticipationType != BattleParticipationType.InviteOnly);

            //still can't open, let's see if it's a participant accessting the page
            if (!canOpen)
            {
                participants = _videoBattleParticipantService.GetVideoBattleParticipants(videoBattleId, null);
                canOpen = participants.Count(x => x.ParticipantId == ApplicationContext.Current.CurrentUser.Id) > 0;
            }
            if (!canOpen)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "get_video_battle");
                return RespondFailure();
            }

            //get all the participants who have been invited, accepted etc. to the battle
            if (participants == null)
                participants = _videoBattleParticipantService.GetVideoBattleParticipants(videoBattleId, null);

            //let's exclude participants who haven't accepted the challenge
            if (videoBattle.VideoBattleStatus != BattleStatus.Pending)
            {
                participants =
                    participants.Where(x => x.ParticipantStatus != BattleParticipantStatus.ChallengeDenied).ToList();
            }

            //lets get the videos associated with battle
            var battleVideos = _videoBattleVideoService.GetBattleVideos(videoBattleId);

            var challengerVideo = battleVideos.FirstOrDefault(x => x.ParticipantId == videoBattle.ChallengerId);
            var challenger = _userService.Get(videoBattle.ChallengerId);

            var challengerVideoModel = new VideoParticipantPublicModel() {
                ParticipantName = challenger.GetPropertyValueAs(PropertyNames.DisplayName, challenger.UserName),
                Id = challenger.Id,
                CanEdit = ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId,
                SeName = challenger.GetPermalink()?.ToString(),
                ParticipantProfileImageUrl =
                    _pictureService.GetPictureUrl(challenger.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId),
                        PictureSizeNames.OriginalProfileImage)
            };

            if (challengerVideo != null &&
                (videoBattle.VideoBattleStatus != BattleStatus.Pending ||
                 challenger.Id == ApplicationContext.Current.CurrentUser.Id))
            {
                challengerVideoModel.VideoPath = challengerVideo.VideoPath;
                // challengerVideoModel.ThumbnailPath = _mobSocialSettings.ShowVideoThumbnailsForBattles ? challengerVideo.ThumbnailPath : "";
                challengerVideoModel.MimeType = challengerVideo.MimeType;
                challengerVideoModel.VideoId = challengerVideo.Id;
                //video is marked watched if 1. participant is viewing his own video 2. he has viewed the video
                challengerVideoModel.VideoWatched = challengerVideo.ParticipantId ==
                                                    ApplicationContext.Current.CurrentUser.Id ||
                                                    _watchedVideoService.IsVideoWatched(
                                                        ApplicationContext.Current.CurrentUser.Id, challengerVideo.Id,
                                                        VideoType.BattleVideo);
            }

            var model = new VideoBattlePublicModel {
                Name = videoBattle.Name,
                Description = videoBattle.Description,
                VotingStartDate = videoBattle.VotingStartDate,
                VotingEndDate = videoBattle.VotingEndDate,
                DateCreated = videoBattle.DateCreated,
                DateUpdated = videoBattle.DateUpdated,
                VideoBattleStatus = videoBattle.VideoBattleStatus,
                BattleParticipationType = videoBattle.ParticipationType,
                VideoBattleVoteType = videoBattle.VideoBattleVoteType,
                Id = videoBattleId,
                RemainingSeconds = videoBattle.GetRemainingSeconds(),
                MaximumParticipantCount = videoBattle.MaximumParticipantCount,
                IsUserLoggedIn = ApplicationContext.Current.CurrentUser.IsRegistered(),
                LoggedInUserId = ApplicationContext.Current.CurrentUser.Id,
                IsVotingPayable = videoBattle.IsVotingPayable,
                CanVoterIncreaseVotingCharge = videoBattle.CanVoterIncreaseVotingCharge,
                MinimumVotingCharge = videoBattle.MinimumVotingCharge,
                ChallengerName = challenger.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                ChallengerSeName = challenger.GetPermalink().ToString(),
                ChallengerProfileImageUrl = challengerVideoModel.ParticipantProfileImageUrl,
                IsSponsorshipSupported = videoBattle.IsSponsorshipSupported,
                MinimumSponsorshipAmount = videoBattle.MinimumSponsorshipAmount,
                VideoBattleSeName = videoBattle.GetPermalink().ToString()
            };

            //add challenger as participant
            model.Participants.Add(challengerVideoModel);

            //and now challengees
            foreach (var participant in participants)
            {
                var challengee = _userService.Get(participant.ParticipantId);
                var cModel = new VideoParticipantPublicModel() {
                    ParticipantName = challengee.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                    SeName = challengee.GetPermalink().ToString(),
                    ParticipantProfileImageUrl =
                        _pictureService.GetPictureUrl(
                            challengee.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId),
                            PictureSizeNames.OriginalProfileImage),
                    Id = challengee.Id,
                    CanEdit = ApplicationContext.Current.CurrentUser.Id == participant.ParticipantId,
                    VideoBattleParticipantStatus = participant.ParticipantStatus,
                    Remarks = participant.Remarks
                };

                //find if the participant has uploaded video? only those who have accepted challege would be shown 'with videos'
                if (participant.ParticipantStatus == BattleParticipantStatus.ChallengeAccepted &&
                    ((videoBattle.VideoBattleStatus != BattleStatus.Pending &&
                      videoBattle.VideoBattleStatus != BattleStatus.Locked) ||
                     participant.ParticipantId == ApplicationContext.Current.CurrentUser.Id))
                {
                    var video = battleVideos.FirstOrDefault(x => x.ParticipantId == participant.ParticipantId);
                    if (video != null)
                    {
                        cModel.VideoPath = video.VideoPath;
                        cModel.MimeType = video.MimeType;
                        //   cModel.ThumbnailPath = _mobSocialSettings.ShowVideoThumbnailsForBattles ? video.ThumbnailPath : "";
                        cModel.VideoId = video.Id;
                        //video is marked watched if 1. participant is viewing his own video 2. he has viewed the video
                        cModel.VideoWatched = video.ParticipantId == ApplicationContext.Current.CurrentUser.Id ||
                                              _watchedVideoService.IsVideoWatched(
                                                  ApplicationContext.Current.CurrentUser.Id, video.Id,
                                                  VideoType.BattleVideo);
                    }
                }
                model.Participants.Add(cModel);
            }

            //let's find if the logged in user has voted for this battle. 
            //also we gather various voting stats till now for battle videos
            //voting info for logged in user
            var videoBattleVotes = _videoBattleVoteService.GetVideoBattleVotes(videoBattleId, null);
            var videoBattleCurrentUserVotes =
                videoBattleVotes.Where(x => x.UserId == ApplicationContext.Current.CurrentUser.Id);

            foreach (var participant in model.Participants)
            {
                //first the global voting status for this participant
                var votesForParticipant =
                    videoBattleVotes.Where(
                        x => x.ParticipantId == participant.Id && x.VoteStatus == BattleVoteStatus.Voted);

                //total votes
                var forParticipant = votesForParticipant as IList<VideoBattleVote> ?? votesForParticipant.ToList();
                if (forParticipant.Count > 0)
                {
                    participant.TotalVoters = forParticipant.Count();

                    //accumulate all vote count
                    model.TotalVotes += participant.TotalVoters;

                    //we store 1 for like 0 for dislike
                    participant.RatingCountLike =
                        forParticipant.Count(x => x.VoteValue == 1);

                    participant.RatingCountDislike =
                        forParticipant.Count(x => x.VoteValue == 0);


                    //average rating is the average of all the vote values 
                    participant.AverageRating = forParticipant.Average(x => (decimal)x.VoteValue);
                }


                //now vote of logged in user
                var currentUserVote =
                    videoBattleCurrentUserVotes.FirstOrDefault(
                        x => x.ParticipantId == participant.Id && x.VoteStatus == BattleVoteStatus.Voted);
                if (currentUserVote != null)
                {
                    //stores the value of vote for logged in user if any
                    participant.CurrentUserVote = new VideoBattleVotePublicModel {
                        VoteValue = currentUserVote.VoteValue
                    };
                }
                else
                {
                    participant.CurrentUserVote = null;
                }
            }

            //and who is the winner or leader?
            VideoParticipantPublicModel winnerOrLeader = null;
            switch (videoBattle.VideoBattleVoteType)
            {
                case BattleVoteType.SelectOneWinner:
                    //one with max vote count is winner/leader
                    winnerOrLeader = model.Participants.OrderByDescending(x => x.TotalVoters).First();

                    break;
                case BattleVoteType.LikeDislike:
                    //one with more likes is winner/leader
                    winnerOrLeader = model.Participants.OrderByDescending(x => x.RatingCountLike).First();
                    break;
                case BattleVoteType.Rating:
                    //one with max average rating is winner/leader
                    winnerOrLeader = model.Participants.OrderByDescending(x => x.AverageRating).First();
                    break;
            }
            if (winnerOrLeader != null && videoBattleVotes.Count > 0)
            {
                winnerOrLeader.IsLeading = true;
                if (videoBattle.VideoBattleStatus == BattleStatus.Complete)
                {
                    winnerOrLeader.IsWinner = true;
                }
            }
            //because we use same interface to inviting participants and voters, its necessary that we show the invite box to participants (who have accepted) as well
            //ofcourse with Invite Voters title
            model.IsParticipant =
                model.Participants.Select(x => x.Id).Contains(ApplicationContext.Current.CurrentUser.Id);
            model.IsEditable = CanEdit(videoBattle);
            model.ViewMode = viewMode;

            //the featured image will be used to display the image on social networks. depending on the status of battle, we either show a default image or 
            //the image of the leader as the featured image 
            model.VideoBattleFeaturedImageUrl = _battleSettings.DefaultVideosFeaturedImageUrl;
            if (model.VideoBattleStatus != BattleStatus.Pending)
            {
                if (winnerOrLeader?.ThumbnailPath != null)
                {
                    model.VideoBattleFeaturedImageUrl = winnerOrLeader.ThumbnailPath;
                }
            }
            //and because the image path starts with ~ (a relative path), we need to convert this to url based on store url
            model.VideoBattleFeaturedImageUrl = model.VideoBattleFeaturedImageUrl;

            var coverId = videoBattle.GetPropertyValueAs<int>(PropertyNames.DefaultCoverId);
            //cover image
            if (coverId != 0)
                model.VideoBattleCoverImageUrl = _pictureService.GetPictureUrl(coverId);

            //now the sponsors, is it supported? let's find out who are the sponsors
            model.IsSponsorshipSupported = videoBattle.IsSponsorshipSupported;
            var sponsors = _sponsorService.GetSponsorsGrouped(null, videoBattle.Id, BattleType.Video, null);

            var sModel =
                sponsors.Select(
                    s => s.ToPublicModel(_userService, _pictureService, _sponsorService, _formatterService,
                            _mediaSettings)).OrderBy(x => x.SponsorData.DisplayOrder).ToList();
            model.Sponsors = sModel.Where(x => x.SponsorshipStatus == SponsorshipStatus.Accepted).ToList();


            //and is the logged in user a sponsor?
            model.CurrentSponsor = sModel.FirstOrDefault(x => x.CustomerId == ApplicationContext.Current.CurrentUser.Id);
            model.IsSponsor = model.CurrentSponsor != null;

            //prizes
            var allPrizes = _videoBattlePrizeService.GetBattlePrizes(videoBattle.Id);
            model.ConsolidatedPrizesDisplay = videoBattle.GetConsolidatedPrizesString(allPrizes.ToList(), null,
                _sponsorService, _settingService, _paymentProcessingService,
                _formatterService, _creditService, _battleSettings);

            //following or not
            model.IsFollowing =
                _followService.GetCustomerFollow<VideoBattle>(ApplicationContext.Current.CurrentUser.Id, videoBattleId) != null ? 1 : 0;
            //and howmany are following
            model.TotalFollowerCount = _followService.GetFollowerCount<VideoBattle>(videoBattleId);

            return RespondSuccess(model);
        }


        [HttpGet]
        [Route("getbattles")]
        public IHttpActionResult GetBattles([FromUri] VideoBattleQueryModel requestModel)
        {
            if (requestModel == null)
            {
                //set default values
                requestModel = new VideoBattleQueryModel();
            }
            if (requestModel.Count == 0)
                requestModel.Count = 15;

            if (requestModel.Page <= 0)
                requestModel.Page = 1;

            if (!requestModel.BattlesSortBy.HasValue)
                requestModel.BattlesSortBy = BattlesSortBy.Id;

            if (!requestModel.SortOrder.HasValue)
                requestModel.SortOrder = SortOrder.Descending;

            if (string.IsNullOrEmpty(requestModel.ViewType))
                requestModel.ViewType = "open";

            //let's get all the battles depending on view type
            IList<VideoBattle> battles = null;
            int totalPages = 0;
            switch (requestModel.ViewType)
            {
                case "open":
                    battles = _videoBattleService.GetAll(null, null, null, BattleStatus.Open, null, null, string.Empty,
                        requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages, requestModel.Page,
                        requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "open-to-join":
                    battles = _videoBattleService.GetAll(null, null, null, BattleStatus.Pending,
                        BattleParticipationType.Open, null, string.Empty, requestModel.BattlesSortBy,
                        requestModel.SortOrder, out totalPages, requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "challenged":
                    battles = _videoBattleService.GetAll(null, ApplicationContext.Current.CurrentUser.Id, null,
                        BattleStatus.Pending, null, null, string.Empty, requestModel.BattlesSortBy,
                        requestModel.SortOrder, out totalPages, requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "closed":
                    //either closed or complete..whichever it is so first get all of them
                    battles = _videoBattleService.GetAll(null, null, null, null, null, null, string.Empty,
                        requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages, 1, int.MaxValue);

                    battles =
                        battles.Where(
                            x =>
                                x.VideoBattleStatus == BattleStatus.Closed ||
                                x.VideoBattleStatus == BattleStatus.Complete)
                            .ToList();
                    totalPages = int.Parse(Math.Ceiling((decimal)battles.Count() / requestModel.Count).ToString());

                    battles = battles.Skip((requestModel.Page - 1) * requestModel.Count)
                        .Take(requestModel.Count).ToList();

                    break;
                case "my":
                    battles = _videoBattleService.GetAll(ApplicationContext.Current.CurrentUser.Id, null, null, null,
                        null, null, string.Empty, requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages,
                        requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "search":
                    battles = _videoBattleService.GetAll(null, null, null, null, BattleParticipationType.Open, null,
                        requestModel.SearchTerm, requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages,
                        requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "sponsor":
                    battles = _videoBattleService.GetAll(ApplicationContext.Current.CurrentUser.Id, null, null, null,
                        null, true, string.Empty, requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages,
                        requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
                case "user":
                    if (requestModel.UserId == 0)
                        requestModel.UserId = ApplicationContext.Current.CurrentUser.Id;
                    battles = _videoBattleService.GetAll(requestModel.UserId, null, null, null, null, true,
                        string.Empty, requestModel.BattlesSortBy, requestModel.SortOrder, out totalPages,
                        requestModel.Page, requestModel.Count);
                    battles = battles.ToList();
                    break;
            }

            var model = new List<VideoBattlePublicModel>();
            if (battles != null)
            {
                foreach (var videoBattle in battles)
                {
                    //get the owner of battle 
                    var challenger = _userService.Get(videoBattle.ChallengerId);
                    if (challenger == null)
                        continue;

                    var battleVideos = _videoBattleVideoService.GetBattleVideos(videoBattle.Id);

                    var thumbnailVideo = battleVideos.FirstOrDefault(x => !string.IsNullOrEmpty(x.ThumbnailPath));

                    var thumbnailUrl = _battleSettings.DefaultVideosFeaturedImageUrl;
                    if (thumbnailVideo != null && videoBattle.VideoBattleStatus != BattleStatus.Pending)
                        thumbnailUrl = thumbnailVideo.ThumbnailPath;

                    //and relative path to url
                    thumbnailUrl = WebHelper.GetUrlFromPath(thumbnailUrl, _generalSettings.ApplicationUiDomain);

                    //prizes
                    var allPrizes = _videoBattlePrizeService.GetBattlePrizes(videoBattle.Id);


                    model.Add(new VideoBattlePublicModel() {
                        Name = videoBattle.Name,
                        Description = videoBattle.Description,
                        VotingStartDate = videoBattle.VotingStartDate,
                        VotingEndDate = videoBattle.VotingEndDate,
                        DateCreated = videoBattle.DateCreated,
                        DateUpdated = videoBattle.DateUpdated,
                        VideoBattleStatus = videoBattle.VideoBattleStatus,
                        BattleParticipationType = videoBattle.ParticipationType,
                        VideoBattleVoteType = videoBattle.VideoBattleVoteType,
                        Id = videoBattle.Id,
                        IsEditable = CanEdit(videoBattle),
                        ChallengerName = challenger.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                        ChallengerSeName =
                            Url.Route("CustomerProfileUrl",
                                new RouteValueDictionary() { { "SeName", challenger.GetPermalink() } }),
                        VideoBattleSeName =
                            Url.Route("VideoBattlePage",
                                new RouteValueDictionary() { { "SeName", videoBattle.GetPermalink() } }),
                        RemainingSeconds = videoBattle.GetRemainingSeconds(),
                        VideoBattleFeaturedImageUrl = thumbnailUrl,
                        ChallengerProfileImageUrl =
                            _pictureService.GetPictureUrl(
                                challenger.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId)),
                        IsSponsorshipSupported = videoBattle.IsSponsorshipSupported,
                        MinimumSponsorshipAmount = videoBattle.MinimumSponsorshipAmount,
                        ConsolidatedPrizesDisplay = videoBattle.GetConsolidatedPrizesString(allPrizes.ToList(), null, _sponsorService, _settingService, _paymentProcessingService, _formatterService, _creditService, _battleSettings)
                    });
                }
            }
            return RespondSuccess(new {
                VideoBattles = model,
                TotalPages = totalPages
            });
        }

        [HttpGet]
        [Route("{videoBattleId:int}/getprizedetails")]
        public IHttpActionResult GetPrizeDetails(int videoBattleId)
        {
            var videoBattle = _videoBattleService.Get(videoBattleId);
            if (videoBattle == null)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "get_prize_details");
                return RespondFailure();
            }

            var battleOwner = _userService.Get(videoBattle.ChallengerId);
            var customerName = "";
            var customerUrl = "";
            if (battleOwner != null)
            {
                customerName = battleOwner.GetPropertyValueAs<string>(PropertyNames.DisplayName);
                customerUrl = Url.Route("CustomerProfileUrl",
                    new RouteValueDictionary()
                    {
                        {"SeName", battleOwner.GetPermalink()}
                    });
            }

            var prizes = _videoBattlePrizeService.GetBattlePrizes(videoBattleId);

            var totalWinnerCount = prizes.Count(x => !x.IsSponsored);

            var model = new List<VideoBattlePrizePublicModel>();

            for (var winnerPosition = 1; winnerPosition <= totalWinnerCount; winnerPosition++)
            {
                var consolidatedString = videoBattle.GetConsolidatedPrizesString(prizes.ToList(), winnerPosition, _sponsorService, _settingService, _paymentProcessingService, _formatterService, _creditService, _battleSettings);

                var prizeModel = new VideoBattlePrizePublicModel() {
                    WinningPosition = "Winner # " + winnerPosition.ToString(),
                    SponsorCustomerUrl = customerUrl,
                    SponsorName = customerName,
                    FormattedPrize = consolidatedString
                };

                model.Add(prizeModel);
            }
            return RespondSuccess(new {
                Prizes = model
            });
        }

        [HttpPost]
        [Authorize]
        [Route("setpictureascover")]
        public IHttpActionResult SetPictureAsCover(int battleId, int pictureId)
        {
            //first get battle & check if it's editable
            var videoBattle = _videoBattleService.Get(battleId);
            if (!CanEdit(videoBattle))
            {
                VerboseReporter.ReportError("Unauthorized", "set_picture_as_cover");
                return RespondFailure();
            }
            //set the property
            videoBattle.SetPropertyValue(PropertyNames.DefaultCoverId, pictureId);
            return RespondSuccess();
        }

        #endregion

        #region Participants

        [HttpPost]
        [Authorize]
        [Route("inviteparticipants")]
        public IHttpActionResult InviteParticipants(InviteParticipantsModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Missing required parameters or invalid data submitted", "save_video_battle");
                return RespondFailure();
            }

            var videoBattleId = requestModel.VideoBattleId;
            var participantIds = requestModel.ParticipantIds;
            var emails = requestModel.Emails;
            //first check if it's a valid videobattle and the logged in user can actually invite
            var videoBattle = _videoBattleService.Get(videoBattleId);
            var model = new List<object>();
            if (CanInvite(videoBattle))
            {
                if (participantIds != null)
                {
                    foreach (var pi in participantIds)
                    {
                        //exclude self
                        if (pi == ApplicationContext.Current.CurrentUser.Id)
                            continue;
                        var status = _videoBattleParticipantService.GetParticipationStatus(videoBattleId, pi);
                        //only people who have not been challenged
                        if (status == BattleParticipantStatus.NotChallenged)
                        {
                            var videoBattleParticipant = new VideoBattleParticipant() {
                                ParticipantId = pi,
                                ParticipantStatus = BattleParticipantStatus.Challenged,
                                VideoBattleId = videoBattleId,
                                LastUpdated = DateTime.UtcNow
                            };
                            _videoBattleParticipantService.Insert(videoBattleParticipant);
                            model.Add(new {
                                Success = true,
                                ParticipantId = pi
                            });

                            //send email notification to the participant
                            var challengee = _userService.Get(pi);
                            _emailSender.SendSomeoneChallengedYouForABattleNotification(ApplicationContext.Current.CurrentUser, challengee, videoBattle);
                        }
                        else
                        {
                            model.Add(new {
                                Success = false,
                                ParticipantId = pi,
                                Status = status
                            });
                        }
                    }
                }

                if (emails != null)
                {
                    //and direct email invites
                    foreach (var email in emails)
                    {
                        _emailSender.SendSomeoneChallengedYouForABattleNotification(ApplicationContext.Current.CurrentUser, email, email, videoBattle);

                        model.Add(new {
                            Success = true,
                            Email = email,
                        });
                    }
                }

                return RespondSuccess(model);
            }
            VerboseReporter.ReportError("Unauthorized", "invite_participants");
            return RespondFailure();
        }

        [HttpPost]
        [Authorize]
        [Route("joinbattle/{videoBattleId:int}")]
        public IHttpActionResult JoinBattle(int videoBattleId)
        {
            //first check if it's a valid videobattle and the logged in user can actually invite
            var videoBattle = _videoBattleService.Get(videoBattleId);


            if (videoBattle == null)
            {
                VerboseReporter.ReportError("Video Battle doesn't exist", "join_battle");
                return RespondFailure();
            }

            //in any case a sponsor can't join a battle
            if (_sponsorService.IsSponsor(ApplicationContext.Current.CurrentUser.Id, videoBattleId, BattleType.Video))
            {
                VerboseReporter.ReportError("Unauthorized", "join_battle");
                return RespondFailure();
            }
            //only open or signup battle types can be joined directly. it should not be open in status either way
            if (videoBattle.ParticipationType != BattleParticipationType.InviteOnly &&
                videoBattle.VideoBattleStatus == BattleStatus.Pending)
            {
                //get the current customer
                var customer = ApplicationContext.Current.CurrentUser;

                //get the participation status
                var status = _videoBattleParticipantService.GetParticipationStatus(videoBattleId, customer.Id);
                //get all participants to get count of total, we can't allow if count reaches a max limit for open battles
                if (videoBattle.ParticipationType == BattleParticipationType.Open)
                {
                    var participants = _videoBattleParticipantService.GetVideoBattleParticipants(videoBattleId,
                        BattleParticipantStatus.ChallengeAccepted);
                    if (participants.Count == videoBattle.MaximumParticipantCount &&
                        videoBattle.MaximumParticipantCount > 0)
                    {
                        //nop, can't join now
                        VerboseReporter.ReportError("No more participants allowed", "join_battle");
                        return RespondFailure();
                    }
                }

                if (status == BattleParticipantStatus.NotChallenged)
                {
                    //not challenged so it's a valid request
                    var videoBattleParticipant = new VideoBattleParticipant {
                        ParticipantId = customer.Id,
                        VideoBattleId = videoBattleId,
                        LastUpdated = DateTime.UtcNow,
                        //depending on the type of battle, the challenge is either accepted directly or marked for approval
                        ParticipantStatus = videoBattle.ParticipationType == BattleParticipationType.Open
                            ? BattleParticipantStatus.ChallengeAccepted
                            : BattleParticipantStatus.SignedUp
                    };


                    //and save it
                    _videoBattleParticipantService.Insert(videoBattleParticipant);

                    //send notification to challenger (the battle host)
                    var challenger = _userService.Get(videoBattle.ChallengerId);
                    var challengee = ApplicationContext.Current.CurrentUser;
                    if (videoBattleParticipant.ParticipantStatus == BattleParticipantStatus.ChallengeAccepted)
                    {
                        //open battle
                        _emailSender.SendVideoBattleJoinNotification(challenger, challengee, videoBattle);
                    }
                    else
                    {
                        _emailSender.SendVideoBattleSignupNotification(challenger, challengee, videoBattle);
                    }

                    //and add user to the followers list
                    _followService.Insert<VideoBattle>(customer.Id, videoBattleId);

                    return RespondSuccess(new { Status = videoBattleParticipant.ParticipantStatus });
                }
            }
            VerboseReporter.ReportError("No more participants allowed", "join_battle");
            return RespondFailure();
        }

        [Authorize]
        [HttpPost]
        [Route("updateparticipantstatus")]
        public IHttpActionResult UpdateParticipantStatus(UpdateParticipantStatusModel model)
        {
            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Missing required parameters or invalid data submitted", "update_participant_status");
                return RespondFailure();
            }

            var videoBattleId = model.BattleId;
            var participantId = model.ParticipantId;
            var videoBattleParticipantStatus = model.VideoBattleParticipantStatus;
            var remarks = model.Remarks;

            var videoBattle = _videoBattleService.Get(videoBattleId);

            if (videoBattle == null)
            {
                //which video battle are you talking about?
                VerboseReporter.ReportError("Video Battle doesn't exist", "update_participant_status");
                return RespondFailure();
            }
            if (ApplicationContext.Current.CurrentUser.Id != participantId &&
                ApplicationContext.Current.CurrentUser.Id != videoBattle.ChallengerId)
            {
                //somebody is not allowed to update status

                VerboseReporter.ReportError("Unauthorized", "update_participant_status");
                return RespondFailure();
            }

            var videoBattleParticipant = _videoBattleParticipantService.GetVideoBattleParticipant(videoBattleId,
                participantId);
            if (videoBattleParticipant == null)
            {
                //uh oh something is wrong here. why is there no video battle participant. was he not invited?
                VerboseReporter.ReportError("Unauthorized", "update_participant_status");
                return RespondFailure();
            }
            //lets first check the battle for validations

            if (videoBattle.VideoBattleStatus != BattleStatus.Pending
                || videoBattle.VotingStartDate < DateTime.UtcNow)
            {
                VerboseReporter.ReportError("Battle closed or expired", "update_participant_status");
                return RespondFailure();
            }


            //so it's there. it must be the person who has signedup for the battle or who has been challenged or who has accepted the challenge and is being disqualified
            if (videoBattleParticipant.ParticipantStatus == BattleParticipantStatus.Challenged
                || videoBattleParticipant.ParticipantStatus == BattleParticipantStatus.SignedUp
                || videoBattleParticipant.ParticipantStatus == BattleParticipantStatus.ChallengeAccepted)
            {
                switch (videoBattleParticipantStatus)
                {
                    case BattleParticipantStatus.ChallengeAccepted:
                    case BattleParticipantStatus.ChallengeDenied:
                        //only the one who is participant should be able to accept/deny the challenge
                        //or if it's an signup battle, battle owner can approve
                        if ((ApplicationContext.Current.CurrentUser.Id == participantId &&
                             videoBattle.ParticipationType != BattleParticipationType.SignUp)
                            ||
                            (ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId &&
                             videoBattle.ParticipationType == BattleParticipationType.SignUp))
                        {
                            videoBattleParticipant.ParticipantStatus = videoBattleParticipantStatus;
                        }
                        break;
                    case BattleParticipantStatus.ChallengeCancelled:
                    case BattleParticipantStatus.Disqualified:
                        //only battle host should be able to mark a participant as disqualified
                        //only the one who challenges or admin can cancel the challenge
                        if (ApplicationContext.Current.CurrentUser.IsAdministrator() ||
                            ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId)
                        {
                            videoBattleParticipant.ParticipantStatus = videoBattleParticipantStatus;
                        }
                        break;
                }
                videoBattleParticipant.LastUpdated = DateTime.UtcNow;
                videoBattleParticipant.Remarks = remarks;
                _videoBattleParticipantService.Update(videoBattleParticipant);

                //send some notifications
                if (ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId &&
                    videoBattle.ParticipationType == BattleParticipationType.SignUp)
                {
                    //this is the request approved by battle owner or admin so notification will be sent to the participant
                    var challengee = _userService.Get(videoBattleParticipant.ParticipantId);
                    var challenger = ApplicationContext.Current.CurrentUser;
                    _emailSender.SendVideoBattleSignupAcceptedNotification(challenger, challengee,
                        videoBattle);
                }
                //disqualified
                if (ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId &&
                    videoBattleParticipantStatus == BattleParticipantStatus.Disqualified)
                {
                    //this is the request approved by battle owner or admin so notification will be sent to the participant
                    var challengee = _userService.Get(videoBattleParticipant.ParticipantId);
                    var challenger = ApplicationContext.Current.CurrentUser;
                    _emailSender.SendVideoBattleDisqualifiedNotification(challenger, challengee,
                        videoBattle);
                }
                return
                    RespondSuccess(
                        new {
                            ParticipantStatus = (int)videoBattleParticipant.ParticipantStatus,
                            ParticipantId = participantId
                        });
            }
            return RespondFailure();
        }

        #endregion

        #region Videos

        /// <summary>
        /// Marks the video battle video watched
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("markvideowatched")]
        public IHttpActionResult MarkVideoWatched(int videoBattleId, int participantId, int videoBattleVideoId)
        {
            //has the user already watched the video?
            if (_watchedVideoService.IsVideoWatched(ApplicationContext.Current.CurrentUser.Id, videoBattleVideoId,
                VideoType.BattleVideo))
            {
                return RespondSuccess();
            }

            //the video must exist before it can be marked watched
            var videoBattleVideo = _videoBattleVideoService.GetBattleVideo(videoBattleId, participantId);
            if (videoBattleVideo == null)
            {
                VerboseReporter.ReportError("Video doesn't exist", "mark_video_watched");
                return RespondFailure();

            }

            //mark the video watched now
            var watchedVideo = new WatchedVideo() {
                CustomerId = ApplicationContext.Current.CurrentUser.Id,
                VideoId = videoBattleVideoId,
                VideoType = VideoType.BattleVideo,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            _watchedVideoService.Insert(watchedVideo);

            return RespondSuccess();
        }

        #endregion

        #region Video Battles Votes

        [Authorize]
        [HttpPost]
        [Route("votebattle")]
        public IHttpActionResult VoteBattle(int videoBattleId, int participantId, int voteValue)
        {
            var user = ApplicationContext.Current.CurrentUser;
            //should not vote himself or herself
            if (user.Id == participantId)
            {
                VerboseReporter.ReportError("You can't vote yourself", "vote_battle");
                return RespondFailure();
            }

            //in any case a sponsor can't vote a battle
            if (_sponsorService.IsSponsor(ApplicationContext.Current.CurrentUser.Id, videoBattleId, BattleType.Video))
            {
                VerboseReporter.ReportError("Sponsors can't vote on a battle", "vote_battle");
                return RespondFailure();
            }

            //has the person watched all the videos before voting can be done
            //get all the videos of current battle
            var battleVideos = _videoBattleVideoService.GetBattleVideos(videoBattleId);
            //and now watched videos
            var watchedVideos = _watchedVideoService.GetWatchedVideos(null, user.Id, VideoType.BattleVideo);


            //if the person voting is already a participant in the battle then one of his own videos need not be watched. 
            var battleVideosIds = battleVideos.Where(x => x.ParticipantId != user.Id).Select(x => x.Id);
            //only current battle videos
            var watchedVideosIds = watchedVideos.Where(x => battleVideosIds.Contains(x.VideoId)).Select(x => x.VideoId);

            if (watchedVideosIds.Count() != battleVideosIds.Count())
            {
                VerboseReporter.ReportError("You haven't watched all videos", "vote_battle");
                return RespondFailure();
            }

            //first find the video battle
            var videoBattle = _videoBattleService.Get(videoBattleId);
            //is the video available for voting
            if (videoBattle.VideoBattleStatus == BattleStatus.Open)
            {
                //check if the logged in user has voted for this battle
                var videoBattleVotes = _videoBattleVoteService.GetVideoBattleVotes(videoBattleId, user.Id);

                var vote =
                    videoBattleVotes.FirstOrDefault(x => x.UserId == user.Id && x.ParticipantId == participantId);

                if (vote != null)
                {
                    //so there is a vote. is the voting status notvoted then only he'll be able to vote
                    if (vote.VoteStatus == BattleVoteStatus.Voted)
                    //already voted for this participant, not it can't be changed
                    {
                        VerboseReporter.ReportError("You have already voted", "vote_battle");
                        return RespondFailure();
                    }

                    //yep...this vote might be from somebody who has been invited and not voted yet. let's do it.
                    vote.VoteValue = voteValue;
                    vote.VoteStatus = BattleVoteStatus.Voted;
                    _videoBattleVoteService.Update(vote);
                }
                else
                {
                    //user has not voted for this participant however it may be possible that depending on vote type, user can't vote on this battle
                    switch (videoBattle.VideoBattleVoteType)
                    {
                        case BattleVoteType.SelectOneWinner:
                            //if one winner was to be selected, we'll have to check if user has not voted for some other participant
                            if (videoBattleVotes.Count(x => x.VoteStatus == BattleVoteStatus.Voted) > 0)
                            {
                                //yes, user has voted for some other participant so he can't vote for this participant now.
                                VerboseReporter.ReportError("You have already voted", "vote_battle");
                                return RespondFailure();
                            }
                            break;
                        case BattleVoteType.Rating:
                            break;
                        case BattleVoteType.LikeDislike:
                            break;
                    }

                    //is this a paid battle, if it is, then user must pay first before proceeding
                    if (videoBattle.IsVotingPayable)
                    {
                        //check if user has available credits to vote
                        var credits = _creditService.GetUsableCreditsCount(user.Id);

                        if (credits < videoBattle.MinimumVotingCharge)
                        {
                            VerboseReporter.ReportError("You don't have enough credits to vote", "vote_battle");
                            return RespondFailure();
                        }

                        //reduce those many credits
                        var credit = new Credit() {
                            CreatedOnUtc = DateTime.UtcNow,
                            CreditCount = videoBattle.MinimumVotingCharge,
                            CreditExchangeRate = _paymentSettings.CreditExchangeRate,
                            CreditTransactionType = CreditTransactionType.Spent,
                            CreditType = CreditType.Transactional,
                            UserId = user.Id,
                            PaymentTransactionId = 0,
                            CreditContextKey = string.Format(CreditContextKeyNames.BattleVote, videoBattle.Id)
                        };
                        _creditService.Insert(credit);
                    }

                    //user can vote now. let's create a new vote
                    var videoBattleVote = new VideoBattleVote() {
                        ParticipantId = participantId,
                        UserId = user.Id,
                        VideoBattleId = videoBattle.Id,
                        VoteValue = voteValue,
                        VoteStatus = BattleVoteStatus.Voted
                    };
                    _videoBattleVoteService.Insert(videoBattleVote);
                }
                //and add user to the followers list
                _followService.Insert<VideoBattle>(user.Id, videoBattleId);

                return RespondSuccess();
            }
            VerboseReporter.ReportError("Closed for voting", "vote_battle");
            return RespondFailure();
        }

        [HttpPost]
        [Authorize]
        [Route("invitevoters")]
        public IHttpActionResult InviteVoters(InviteVotersModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Missing required parameters or invalid data submitted", "invite_voters");
                return RespondFailure();
            }

            var videoBattleId = requestModel.VideoBattleId;
            var voterIds = requestModel.VoterIds;
            var emails = requestModel.Emails;
            //first check if it's a valid videobattle and the logged in user can actually invite
            var videoBattle = _videoBattleService.Get(videoBattleId);
            if (videoBattle == null)
            {
                VerboseReporter.ReportError("Battle doesn't exist", "invite_voters");
                return RespondFailure();

            }

            var participants = _videoBattleParticipantService.GetVideoBattleParticipants(videoBattleId,
                BattleParticipantStatus.ChallengeAccepted);

            var model = new List<object>();
            if (CanInvite(videoBattle) ||
                participants.Select(x => x.ParticipantId).Contains(ApplicationContext.Current.CurrentUser.Id))
            {
                if (voterIds != null)
                {
                    var votes = _videoBattleVoteService.GetVideoBattleVotes(videoBattleId, null);

                    foreach (var vi in voterIds)
                    {
                        //exclude self
                        if (vi == ApplicationContext.Current.CurrentUser.Id)
                            continue;
                        var vote = votes.FirstOrDefault(x => x.UserId == vi);
                        if (vote == null)
                        {
                            vote = new VideoBattleVote() {
                                VideoBattleId = videoBattleId,
                                ParticipantId = ApplicationContext.Current.CurrentUser.Id,
                                VoteStatus = BattleVoteStatus.NotVoted,
                                VoteValue = 0,
                                UserId = vi
                            };
                            _videoBattleVoteService.Insert(vote);

                            //send the notification
                            var receiver = _userService.Get(vi);
                            _emailSender.SendVotingReminderNotification(
                                ApplicationContext.Current.CurrentUser, receiver,
                                videoBattle);
                            model.Add(new {
                                Success = true,
                                VoterId = vi,
                            });
                        }
                        else
                        {
                            model.Add(new {
                                Success = false,
                                VoterId = vi,
                                Message = "Already invited"
                            });
                        }
                    }
                }

                if (emails != null)
                {
                    //and direct email invites
                    foreach (var email in emails)
                    {
                        _emailSender.SendVotingReminderNotification(
                            ApplicationContext.Current.CurrentUser, email, email, videoBattle);
                        model.Add(new {
                            Success = true,
                            Email = email,
                        });
                    }
                }
                return RespondSuccess(model);
            }
            VerboseReporter.ReportError("Unauthorized", "invite_voters");
            return RespondFailure();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Checks if current logged in user can actually edit the battle
        /// </summary>
        /// <returns>True if editing is allowed. False otherwise</returns>
        [NonAction]
        bool CanEdit(VideoBattle videoBattle)
        {
            if (videoBattle == null)
                return false;
            return (ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId //page owner
                    || videoBattle.Id == 0 //new battle
                    || ApplicationContext.Current.CurrentUser.IsAdministrator()); //administrator
        }

        /// <summary>
        /// Checks if current logged in user can actually delete the battle
        /// </summary>
        /// <returns>True if deletion is allowed. False otherwise</returns>
        [NonAction]
        bool CanDelete(VideoBattle videoBattle)
        {
            if (videoBattle == null)
                return false;
            return ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId //page owner
                   || ApplicationContext.Current.CurrentUser.IsAdministrator(); //administrator
        }

        /// <summary>
        /// Checks if current logged in user can actually invite a participant
        /// </summary>
        /// <returns>True if invite is allowed. False otherwise</returns>
        [NonAction]
        bool CanInvite(VideoBattle videoBattle)
        {
            if (videoBattle == null)
                return false;
            return videoBattle.VideoBattleStatus != BattleStatus.Complete
                   && videoBattle.VideoBattleStatus != BattleStatus.Closed
                   &&
                   ((ApplicationContext.Current.CurrentUser.Id == videoBattle.ChallengerId &&
                     videoBattle.ParticipationType == BattleParticipationType.InviteOnly) //page owner
                    || videoBattle.ParticipationType == BattleParticipationType.Open
                    || ApplicationContext.Current.CurrentUser.IsAdministrator()); //administrator
        }

        #endregion
    }
}