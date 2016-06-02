using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Credits;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Data.Enum;
using mobSocial.Services.Battles;
using mobSocial.Services.Credits;
using mobSocial.Services.Emails;
using mobSocial.Services.Extensions;
using mobSocial.Services.Formatter;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Sponsors;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions;
using mobSocial.WebApi.Models.Battles;
using mobSocial.WebApi.Models.Sponsors;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("sponsors")]
    public class SponsorController : RootApiController
    {
        #region Fields

        private readonly ISponsorService _sponsorService;
        private readonly IVideoBattleService _videoBattleService;
        private readonly IVideoBattlePrizeService _videoBattlePrizeService;
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly IFormatterService _formatterService;
        private readonly IEmailSender _emailSender;
        private readonly ICreditService _creditService;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region ctor
        public SponsorController(ISponsorService sponsorService,
            IVideoBattleService videoBattleService,
            IUserService customerService,
            IMediaService mediaService,
            IFormatterService formatterService,
            MediaSettings mediaSettings,
            IEmailSender emailSender,
            IVideoBattlePrizeService videoBattlePrizeService,
            ICreditService creditService)
        {
            _sponsorService = sponsorService;
            _videoBattleService = videoBattleService;
            _userService = customerService;
            _mediaService = mediaService;
            _formatterService = formatterService;
            _mediaSettings = mediaSettings;
            _emailSender = emailSender;
            _videoBattlePrizeService = videoBattlePrizeService;
            _creditService = creditService;
        }
        #endregion



        #region Methods

        [HttpPost]
        [Authorize]
        [Route("savesponsor")]
        public IHttpActionResult SaveSponsor(SponsorModel requestModel)
        {
            if (!ModelState.IsValid)
                return Json(new { Success = false, Message = "Invalid Data" });

            var battle = _videoBattleService.Get(requestModel.BattleId); //todo: query picture battles when ready
            if (battle == null)
                return Json(new { Success = false, Message = "Invalid Battle" });

            var isProductOnlySponsorship = requestModel.SponsorshipType == SponsorshipType.OnlyProducts;

            var userId = ApplicationContext.Current.CurrentUser.Id;

            //there is a possibility that a sponsor is increasing the sponsorship amount. In that case, the new sponsorship status will automatically
            //be of same status as that of previous one for the same battle
            //lets find if the sponsor already exist for this battle?
            var existingSponsors = _sponsorService.GetSponsors(userId, requestModel.BattleId, requestModel.BattleType, null);

            var newSponsorStatus = SponsorshipStatus.Pending;

            //so is the current customer already an sponsor for this battle?
            if (existingSponsors.Any())
                newSponsorStatus = existingSponsors.First().SponsorshipStatus;

            if (!isProductOnlySponsorship)
            {
                //are issued credits sufficient?
                var issuedCreditsCount = _creditService.GetUsableCreditsCount(userId);
                if ((issuedCreditsCount < battle.MinimumSponsorshipAmount && newSponsorStatus != SponsorshipStatus.Accepted) || requestModel.SponsorshipCredits > issuedCreditsCount)
                {
                    VerboseReporter.ReportError("Insufficient credits to become sponsor", "save_sponsor");
                    return RespondFailure();
                }
            }

            //mark the credits as spent credits
            var spentCredit = new Credit() {
                CreditContextKey = string.Format(CreditContextKeyNames.BattleSponsor, battle.Id),
                CreditCount = requestModel.SponsorshipCredits,
                CreditTransactionType = CreditTransactionType.Spent,
                CreditType = CreditType.Transactional,
                PaymentTransactionId = 0,
                CreatedOnUtc = DateTime.UtcNow,
                UserId = userId
            };
            _creditService.Insert(spentCredit);

            //create the sponsor and set the status to pending or an existing status as determined above. (the status will be marked accepted or rejected or cancelled by battle owner)
            var sponsor = new Sponsor() {
                BattleId = requestModel.BattleId,
                BattleType = requestModel.BattleType,
                SponsorshipAmount = isProductOnlySponsorship ? 0 : requestModel.SponsorshipCredits,
                UserId = userId,
                SponsorshipStatus = newSponsorStatus,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };
            //save the sponsor
            _sponsorService.Insert(sponsor);

            if (!isProductOnlySponsorship)
            {
                //save the prizes only sponsorship prizes
                SaveSponsorProductPrizes(requestModel.Prizes);
            }


            var battleOwner = _userService.Get(battle.ChallengerId);

            var sponsorCustomer = _userService.Get(userId);

            //send notification to battle owner
            _emailSender.SendSponsorAppliedNotificationToBattleOwner(battleOwner, sponsorCustomer, battle);

            return Json(new { Success = true });
        }

        [HttpPost]
        [Authorize]
        [Route("updatesponsor")]
        public IHttpActionResult UpdateSponsor(UpdateSponsorModel requestModel)
        {
            //here we update or reject the status of battle
            if (!ModelState.IsValid)
                return Json(new { Success = false, Message = "Invalid Data" });

            //because a sponsor may have performed multiple transactions for a battle, we'd update all of them at once 

            //first get the battle
            //TODO: Remove comment when picture battles are ready
            var battle = _videoBattleService.Get(requestModel.BattleId); // Model.BattleType == BattleType.Video ? _videoBattleService.GetById(Model.BattleId) : null;

            //only battle owner should be able to accept or reject the sponsorship
            if (battle.ChallengerId != ApplicationContext.Current.CurrentUser.Id && requestModel.SponsorshipStatus != SponsorshipStatus.Cancelled)
                return Json(new { Success = false, Message = "Unauthorized" });

            //similarly only the sponsor should be able to withdraw it
            if (requestModel.SponsorCustomerId != ApplicationContext.Current.CurrentUser.Id && requestModel.SponsorshipStatus == SponsorshipStatus.Cancelled)
                return Json(new { Success = false, Message = "Unauthorized" });

            var sponsorCustomer = _userService.Get(requestModel.SponsorCustomerId);
            if (sponsorCustomer == null)
            {
                VerboseReporter.ReportError("Invalid customer Id", "update_sponsor");
                return RespondFailure();
            }

            if (requestModel.SponsorshipStatus == SponsorshipStatus.Accepted)
            {
                //update sponsorship status
                _sponsorService.UpdateSponsorStatus(sponsorCustomer.Id, battle.Id, BattleType.Video, SponsorshipStatus.Accepted);
            }
            else if (requestModel.SponsorshipStatus == SponsorshipStatus.Cancelled || requestModel.SponsorshipStatus == SponsorshipStatus.Rejected)
            {
                //update sponsorship status
                _sponsorService.UpdateSponsorStatus(sponsorCustomer.Id, battle.Id, BattleType.Video, requestModel.SponsorshipStatus);

                //in case the sponsorship is cancelled or rejected, the appropriate number of credits should be refunded to the sponsor user
                //to get the total sponsorship credits, let's first query
                var sponsorships = _sponsorService.GetSponsors(sponsorCustomer.Id, battle.Id, BattleType.Video, null);
                //sum all the sponsorships to get total credits
                var totalCredits = sponsorships.Sum(x => x.SponsorshipAmount);

                var refundCredit = new Credit() {
                    CreatedOnUtc = DateTime.UtcNow,
                    CreditTransactionType = CreditTransactionType.Refund,
                    CreditContextKey = string.Format(CreditContextKeyNames.BattleSponsor, battle.Id),
                    CreditCount = totalCredits,
                    CreditType = CreditType.Transactional,
                    Remarks = $"Refund towards cancellation of sponsorship for video battle '{battle.Name}'",
                    UserId = sponsorCustomer.Id
                };

                //save the credit
                _creditService.Insert(refundCredit);

                //send sponsorship update status to battle owner and admin
                var user = _userService.Get(requestModel.SponsorshipStatus == SponsorshipStatus.Cancelled ? battle.ChallengerId : requestModel.SponsorCustomerId);

                //send notification
                _emailSender.SendSponsorshipStatusChangeNotification(user, requestModel.SponsorshipStatus, battle);
            }
            return RespondSuccess();
        }

        [HttpPost]
        [Route("savesponsordata")]
        public IHttpActionResult SaveSponsorData(SponsorDataModel requestModel)
        {
            //here we update or reject the status of battle
            if (!ModelState.IsValid)
                return Json(new { Success = false, Message = "Invalid Data" });

            //because a sponsor may have performed multiple transactions for a battle, we'd update all of them at once 

            //first get the battle
            //TODO: Remove comment when picture battles are ready
            var battle = _videoBattleService.Get(requestModel.BattleId); // Model.BattleType == BattleType.Video ? _videoBattleService.GetById(Model.BattleId) : null;

            //first get battle
            var videoBattle = _videoBattleService.Get(requestModel.BattleId);
            var sponsors = _sponsorService.GetSponsors(ApplicationContext.Current.CurrentUser.Id, requestModel.BattleId, BattleType.Video,
                SponsorshipStatus.Accepted);
            //only the battle owner or the sponsor can save the sponsor data
            if (!(sponsors.Any() || videoBattle.ChallengerId == ApplicationContext.Current.CurrentUser.Id))
                return Json(new { Success = false, Message = "Unauthorized" });

            //get the sponsor data. Because battle owner can also save the data, we need to make sure that correct data is requested
            //because battle owner can save anybody's data while sponsor can save only his or her data
            var sponsorData = _sponsorService.GetSponsorData(requestModel.BattleId, requestModel.BattleType, sponsors.Any() ? ApplicationContext.Current.CurrentUser.Id : requestModel.SponsorCustomerId);
            sponsorData.DisplayName = requestModel.DisplayName;
            sponsorData.PictureId = requestModel.PictureId;
            sponsorData.TargetUrl = requestModel.TargetUrl;
            sponsorData.DateUpdated = DateTime.UtcNow;

            //display order can only be changed by battle owner depending on the amount or his choice
            if (videoBattle.ChallengerId == ApplicationContext.Current.CurrentUser.Id)
                sponsorData.DisplayOrder = requestModel.DisplayOrder;

            _sponsorService.SaveSponsorData(sponsorData);

            return Json(new { Success = true });

        }

        [HttpGet]
        [Authorize]
        [Route("getsponsors")]
        public IHttpActionResult GetSponsors([FromUri] SponsorsRequestModel requestModel)
        {
            //first we check if it's the battle owner or a sponsor calling this method?
            //for that we need to query the battle first
            //todo: get picture battle when ready
            var battle = requestModel.BattleType == BattleType.Video ? _videoBattleService.Get(requestModel.BattleId) : null;

            if (battle == null)
                return Json(new { Success = false, Message = "Battle doesn't exist" });

            //lets query the sponsor
            var sponsors = battle.ChallengerId == ApplicationContext.Current.CurrentUser.Id
                ? _sponsorService.GetSponsorsGrouped(null, requestModel.BattleId, requestModel.BattleType, requestModel.SponsorshipStatus) //battle owner
                : _sponsorService.GetSponsorsGrouped(ApplicationContext.Current.CurrentUser.Id, requestModel.BattleId, requestModel.BattleType, requestModel.SponsorshipStatus); //sponsor or somebody else?

            //to list
            var model = sponsors.Select(s => s.ToPublicModel(_userService, _mediaService, _sponsorService, _formatterService, _mediaSettings)).OrderBy(x => x.SponsorData.DisplayOrder).ToList();

            var allPrizes = _videoBattlePrizeService.GetBattlePrizes(requestModel.BattleId);
            var totalWinningPositions = allPrizes.Count(x => !x.IsSponsored);

            //and do we have any existing saved prizes which are sponsored
            foreach (var m in model)
            {
                m.SponsoredProductPrizes = new List<VideoBattlePrizeModel>();
                var sponsoredPrizes = allPrizes.Where(x => x.IsSponsored && m.CustomerId == x.SponsorCustomerId);
                foreach (var prize in sponsoredPrizes)
                {
                    var prizeModel = new VideoBattlePrizeModel() {
                        PrizeType = prize.PrizeType,
                        PrizeOther = prize.PrizeOther,
                        WinnerPosition = prize.WinnerPosition,
                        Id = prize.Id,
                        IsSponsored = prize.IsSponsored,
                        VideoBattleId = battle.Id,
                        SponsorCustomerId = prize.SponsorCustomerId
                    };
                    m.SponsoredProductPrizes.Add(prizeModel);
                }
                var totalSponsoredPrizes = sponsoredPrizes.Count();
                //if not all winning positions have been covered, add the remaining
                for (var index = totalSponsoredPrizes + 1; index <= totalWinningPositions; index++)
                {
                    m.SponsoredProductPrizes.Add(new VideoBattlePrizeModel() {
                        Id = 0,
                        PrizeType = BattlePrizeType.Other,
                        WinnerPosition = index,
                        IsSponsored = true,
                        PrizeOther = "",
                        VideoBattleId = battle.Id,
                        SponsorCustomerId = m.CustomerId
                    });
                }
            }

            return Json(new {
                Success = true,
                Sponsors = model,
                IsChallenger = battle.ChallengerId == ApplicationContext.Current.CurrentUser.Id,
                IsSponsor = model.Any(x => x.CustomerId == ApplicationContext.Current.CurrentUser.Id),
                BattleName = battle.Name,
                BattleUrl = Url.Route("VideoBattlePage", new RouteValueDictionary()
                {
                  {"SeName",  battle.GetPermalink()}
                }),
                TotalWinningPositions = totalWinningPositions
            });
        }

        [HttpGet]
        [Authorize]
        [Route("getsponsortransactions")]
        public IHttpActionResult GetSponsorTransactions([FromUri] SponsorTransactionRequestModel requestModel)
        {
            //only battle owner or sponsor should be able to see transactions
            //todo: get picture battle when ready
            var battle = requestModel.BattleType == BattleType.Video ? _videoBattleService.Get(requestModel.BattleId) : null;
            if (battle == null)
                return Json(new { Success = false, Message = "Battle doesn't exist" });


            if (battle.ChallengerId != ApplicationContext.Current.CurrentUser.Id &&
                requestModel.CustomerId != ApplicationContext.Current.CurrentUser.Id)
                return Json(new { Success = false, Message = "Unauthorized" });

            var sponsorships = _sponsorService.GetSponsors(requestModel.CustomerId, requestModel.BattleId, BattleType.Video, null);
            var model = new List<SponsorTransactionModel>();
            var user = _userService.Get(requestModel.CustomerId);

            foreach (var sponsorship in sponsorships)
            {
                model.Add(new SponsorTransactionModel() {
                    OrderId = sponsorship.Id,
                    TransactionDate = DateTimeHelper.GetDateInUserTimeZone(sponsorship.DateCreated, DateTimeKind.Utc, user).ToString(),
                    TransactionAmount = sponsorship.SponsorshipAmount,
                    TransactionAmountFormatted = _formatterService.FormatCurrency(sponsorship.SponsorshipAmount, ApplicationContext.Current.ActiveCurrency)
                });
            }

            return Json(new { Success = true, Orders = model });


        }


        [HttpPost]
        [Authorize]
        public IHttpActionResult ProductPrizesFormPopup(int battleId, BattleType battleType)
        {
            //check if current user is already a sponsor, he should then be doing everything from sponsor dashboard only
            if (_sponsorService.GetSponsors(ApplicationContext.Current.CurrentUser.Id, battleId, battleType, null).Any())
            {
                return null;
            }

            var allPrizes = _videoBattlePrizeService.GetBattlePrizes(battleId);
            var totalWinningPositions = allPrizes.Count(x => !x.IsSponsored);

            var model = new List<VideoBattlePrizeModel>();
            for (var index = 1; index <= totalWinningPositions; index++)
            {
                model.Add(new VideoBattlePrizeModel() {
                    WinnerPosition = index,
                    PrizeType = BattlePrizeType.Other,
                    IsSponsored = true,
                    PrizeOther = "",
                    SponsorCustomerId = ApplicationContext.Current.CurrentUser.Id,
                    VideoBattleId = battleId
                });
            }
            return Json(model);
        }

        [HttpPost]
        [Authorize]
        [Route("savesponsorproductprizes")]
        public IHttpActionResult SaveSponsorProductPrizes(IList<VideoBattlePrizeModel> models)
        {
            if (!models.Any())
                return Json(new { Success = false });

            //for performance reasons, it's better to first query all the battle prizes
            var battleId = models.First().VideoBattleId;
            var sponsorId = models.First().SponsorCustomerId;

            var battle = _videoBattleService.Get(battleId);

            var sponsors = _sponsorService.GetSponsors(ApplicationContext.Current.CurrentUser.Id, battleId, BattleType.Video, null);

            if (!sponsors.Any() && battle.ChallengerId != ApplicationContext.Current.CurrentUser.Id)
                return Json(new { Success = false, Message = "Unauthorized" });

            var allPrizes = _videoBattlePrizeService.GetBattlePrizes(battleId);

            //filter sponsored prizes
            var sponsoredPrizes = allPrizes.Where(x => x.IsSponsored);

            //and if it's the sponsor who is logged in
            if (sponsorId == ApplicationContext.Current.CurrentUser.Id)
                sponsoredPrizes = sponsoredPrizes.Where(x => x.SponsorCustomerId == ApplicationContext.Current.CurrentUser.Id);

            //now loop through the model and save each as and when required
            foreach (var model in models)
            {
                //exclude empty models
                if (string.IsNullOrEmpty(model.PrizeOther))
                    continue;

                var prize = sponsoredPrizes.FirstOrDefault(x => x.Id == model.Id) ?? new VideoBattlePrize();

                prize.IsSponsored = true;
                prize.SponsorCustomerId = model.SponsorCustomerId;
                prize.PrizeType = BattlePrizeType.Other;
                prize.PrizeOther = model.PrizeOther;
                prize.WinnerPosition = model.WinnerPosition;
                prize.VideoBattleId = battleId;
                prize.DateUpdated = DateTime.UtcNow;
                if (prize.Id == 0)
                {
                    prize.DateCreated = DateTime.UtcNow;
                    _videoBattlePrizeService.Insert(prize);
                }
                else
                    _videoBattlePrizeService.Update(prize);
            }
            return Json(new { Success = true });
        }

       #endregion
      
    }
}