using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Sponsors
{
    public interface ISponsorPassService : IBaseEntityService<SponsorPass>
    {
        Order GetSponsorPassOrder(int sponsorPassOrderId);

        IList<Order> GetSponsorPassOrders(int sponsorCustomerId, int battleId, BattleType battleType);

        IList<SponsorPass> GetPurchasedSponsorPasses(int customerId, PassStatus? sponsorPassStatus);

        int CreateSponsorPass(BattleType battleType, int battleId, MobSocialProcessPaymentResultModel paymentResponse, UserPaymentMethod paymentMethod, decimal amount);

        void MarkSponsorPassUsed(int sponsorPassOrderId, int battleId, BattleType battleType);

        SponsorPass GetSponsorPassByOrderId(int orderId);


    }
}