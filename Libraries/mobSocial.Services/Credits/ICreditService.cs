using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Credits;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Credits
{
    public interface ICreditService : IBaseEntityService<Credit>
    {
        /// <summary>
        /// Gets total available credit count
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="creditType"></param>
        /// <returns></returns>
        decimal GetAvailableCreditsCount(int userId, CreditType? creditType);

        /// <summary>
        /// Gets total available credits count
        /// </summary>
        /// <param name="contextKeyName"></param>
        /// <param name="creditType"></param>
        /// <param name="creditTransactionType"></param>
        /// <returns></returns>
        IList<Credit> GetCredits(string contextKeyName, CreditType? creditType, CreditTransactionType? creditTransactionType);

        /// <summary>
        /// Gets the total available credit amount using their respective exchange rates
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="creditType"></param>
        /// <returns></returns>
        decimal GetAvailableCreditsAmount(int userId, CreditType? creditType);

        /// <summary>
        /// Gets usable credit count for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        decimal GetUsableCreditsCount(int userId);
    }
}