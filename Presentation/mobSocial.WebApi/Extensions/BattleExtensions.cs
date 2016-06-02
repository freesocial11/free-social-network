using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.Services.Battles;
using mobSocial.Services.Credits;
using mobSocial.Services.Formatter;
using mobSocial.Services.Helpers;
using mobSocial.Services.Payments;
using mobSocial.Services.Settings;
using mobSocial.Services.Sponsors;
using mobSocial.WebApi.Configuration.Infrastructure;

namespace mobSocial.WebApi.Extensions
{
    public static class BattleExtensions
    {
        public static string GetConsolidatedPrizesString(this VideoBattle battle, 
            List<VideoBattlePrize> allPrizes, 
            int? winnerPosition,
            ISponsorService sponsorService,
            ISettingService settingService,
            IPaymentProcessingService paymentProcessingService,
            IFormatterService formatterService,
            ICreditService creditService,
            BattleSettings battleSettings)
        {

            var battleOwnerPrizes = allPrizes.Where(x => !x.IsSponsored  && (!winnerPosition.HasValue || x.WinnerPosition == winnerPosition.Value));
            var sponsoredPrizes = allPrizes.Where(x => x.IsSponsored && (!winnerPosition.HasValue || x.WinnerPosition == winnerPosition.Value));

            var videoBattlePrizes = battleOwnerPrizes as VideoBattlePrize[] ?? battleOwnerPrizes.ToArray();
            var totalPrizesAmountFixed = videoBattlePrizes.Where(x => x.PrizeType == BattlePrizeType.FixedAmount).Sum(x => x.PrizeAmount);

            var totalPrizesAmountPercentage = 0m;
            if (videoBattlePrizes.Any(x => x.PrizeType == BattlePrizeType.PercentageAmount))
            {
                var contextKeyName = string.Format(CreditContextKeyNames.BattleVote, battle.Id);
                //get spent credits for battle votes
                var credits = creditService.GetCredits(contextKeyName, null, CreditTransactionType.Spent);

                var orderSum = credits.Sum(x => x.CreditCount);
                var netOrderSum = paymentProcessingService.GetNetAmountAfterPaymentProcessing(orderSum);

                var totalWinners = videoBattlePrizes.Count();

                //total voting amount from percentage
                totalPrizesAmountPercentage = netOrderSum -
                                              totalWinners*netOrderSum*battle.ParticipantPercentagePerVote/100;



            }

            var sponsorships = sponsorService.GetSponsorsGrouped(null, battle.Id, BattleType.Video,
                SponsorshipStatus.Accepted);

            var sponsorshipAmount = sponsorships.Sum(x => x.SponsorshipAmount);

            //amount after payment processing
            var netSponsorshipAmount = paymentProcessingService.GetNetAmountAfterPaymentProcessing(sponsorshipAmount);

            var siteOwnerShare = netSponsorshipAmount * battleSettings.SiteOwnerVideoBattleSponsorshipPercentage / 100;

            //it may be possible that battle host himself is sponsor, he won't be getting commissions for that :)
            var battleHostAsSponsorAmount =
                sponsorships.Where(x => x.UserId == battle.ChallengerId).Sum(x => x.SponsorshipAmount);


            var battleHostShare = (netSponsorshipAmount - battleHostAsSponsorAmount) * battleSettings.BattleHostVideoBattleSponsorshipPercentage / 100;

            //amount available for winners
            var winnerPrizePool = netSponsorshipAmount - siteOwnerShare - battleHostShare;

            if (winnerPosition.HasValue)
            {
                winnerPrizePool = PrizeDistributionHelper.GetPrizeDistributionPercentage(winnerPosition.Value,
                    allPrizes.Count(x => !x.IsSponsored), settingService) * winnerPrizePool;
            }

            var totalAmount = Math.Round(totalPrizesAmountFixed + totalPrizesAmountPercentage + winnerPrizePool);

            var totalPrizeString = totalAmount > 0 ? formatterService.FormatCurrency(totalAmount, ApplicationContext.Current.ActiveCurrency) : "";


            if (allPrizes.Any(x => x.PrizeType == BattlePrizeType.FixedProduct || x.PrizeType == BattlePrizeType.Other))
            {
                if (!winnerPosition.HasValue)
                    totalPrizeString += "+";
                else
                {
                    //now append each product as prize with it's name to the prize string
                    foreach (var prize in videoBattlePrizes.Where(x => x.PrizeType == BattlePrizeType.FixedProduct || x.PrizeType == BattlePrizeType.Other))
                    {
                        if (prize.PrizeType == BattlePrizeType.FixedProduct)
                        {
                            //todo: do something for fixed product
                        }
                        else
                        {
                            totalPrizeString += (totalPrizeString != "" ? " + " : "") + prize.PrizeOther;
                        }
                    }
                    //and sponsored products
                    foreach (var prize in sponsoredPrizes.Where(x => x.PrizeType == BattlePrizeType.Other))
                    {
                        totalPrizeString += (totalPrizeString != "" ? " + " : "") + prize.PrizeOther + "*";
                    }
                }
            }

            return totalPrizeString;
        }

    }
}