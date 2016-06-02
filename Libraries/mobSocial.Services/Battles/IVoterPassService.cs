using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Enum;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace mobSocial.Services.Battles
{
    public interface IVoterPassService : IBaseEntityService<VoterPass>
    {
        Order GetVoterPassOrder(int voterPassOrderId);

        IList<VoterPass> GetPurchasedVoterPasses(int customerId, PassStatus? votePassStatus);

        int CreateVoterPass(BattleType battleType, int battleId, MobSocialProcessPaymentResultModel paymentResponse, UserPaymentMethod paymentMethod, decimal amount);

        void MarkVoterPassUsed(int voterPassOrderId);

        VoterPass GetVoterPassByOrderId(int orderId);

        IList<Order> GetAllVoterPassOrders(BattleType battleType, int battleId, PassStatus? voterPassStatus);
    }
}