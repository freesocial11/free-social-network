using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Credits;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Credits
{
    public class CreditService : MobSocialEntityService<Credit>, ICreditService
    {
        private readonly PaymentSettings _paymentSettings;
        public CreditService(IDataRepository<Credit> dataRepository, PaymentSettings paymentSettings) : base(dataRepository)
        {
            _paymentSettings = paymentSettings;
        }

        public decimal GetAvailableCreditsCount(int userId, CreditType? creditType)
        {
            //get all credits first (exclude expired or locked ones)
            var credits = Get(x => x.UserId == userId && (creditType == null || creditType == x.CreditType) && !x.IsExpired && DateTime.UtcNow < x.ExpiresOnUtc, null);

            //earned credits
            var earned = credits.Where(x => x.CreditTransactionType == CreditTransactionType.Issued).Sum(x => x.CreditCount);

            //spent credits
            var spent = credits.Where(x => x.CreditTransactionType == CreditTransactionType.Spent).Sum(x => x.CreditCount);

            //refund credits
            var refund = credits.Where(x => x.CreditTransactionType == CreditTransactionType.Refund).Sum(x => x.CreditCount);

            //reversed credits
            var reversed = credits.Where(x => x.CreditTransactionType == CreditTransactionType.IssuedRevert).Sum(x => x.CreditCount);

            var consolidated = earned + refund - spent - reversed;
            return consolidated;
        }

        public IList<Credit> GetCredits(string contextKeyName, CreditType? creditType, CreditTransactionType? creditTransactionType)
        {
            //first get the credits
            var credits =
                Get(
                    x =>
                        x.CreditContextKey == contextKeyName &&
                        (!creditType.HasValue || x.CreditType == creditType.Value) &&
                        (!creditTransactionType.HasValue || x.CreditTransactionType == creditTransactionType)
                        , null);

            return credits.ToList();
        }

        public decimal GetAvailableCreditsAmount(int userId, CreditType? creditType)
        {
            //get all credits first
            var credits = Get(x => x.UserId == userId && (creditType == null || creditType == x.CreditType), null);

            //earned credits
            var earned =
                credits.Where(x => x.CreditTransactionType == CreditTransactionType.Issued).Select(x => x.CreditCount*x.CreditExchangeRate).Sum();

            //spent credits
            var spent = credits.Where(x => x.CreditTransactionType == CreditTransactionType.Spent).Select(x => x.CreditCount * x.CreditExchangeRate).Sum();

            //refund credits
            var refund = credits.Where(x => x.CreditTransactionType == CreditTransactionType.Refund).Select(x => x.CreditCount * x.CreditExchangeRate).Sum();

            //reversed credits
            var reversed = credits.Where(x => x.CreditTransactionType == CreditTransactionType.IssuedRevert).Select(x => x.CreditCount * x.CreditExchangeRate).Sum();

            var consolidated = earned + refund - spent - reversed;
            return consolidated;
        }

        public decimal GetUsableCreditsCount(int userId)
        {
            var transactionalCredits = GetAvailableCreditsCount(userId, CreditType.Transactional);

            var promotionalCredits = GetAvailableCreditsCount(userId, CreditType.Promotional);

            var promotionalCreditLimitNumber = _paymentSettings.PromotionalCreditUsageLimitPerTransaction;
            var usablePromotionalCreditCount = (int)(_paymentSettings.IsPromotionalCreditUsageLimitPercentage
                ? (promotionalCredits * promotionalCreditLimitNumber * 0.01m)
                : promotionalCreditLimitNumber);

            //so usable credits are transactional + usablepromotional
            return transactionalCredits + usablePromotionalCreditCount;
        }
    }
}