using System;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Helpers
{
    public static class VideoBattleHelpers
    {
        /// <summary>
        /// Returns remaining seconds of a battle for opening the battle (if it's pending) or completing the battle (if it's locked)
        /// </summary>
        /// <returns></returns>
        public static int GetRemainingSeconds(this VideoBattle videoBattle)
        {
            var now = DateTime.UtcNow;
            var endDate = DateTime.UtcNow;

            if (videoBattle.VideoBattleStatus == BattleStatus.Pending && videoBattle.VotingStartDate > now)
            {
                endDate = videoBattle.VotingStartDate;
            }
            else if (videoBattle.VideoBattleStatus == BattleStatus.Open)
            {
                endDate = videoBattle.VotingEndDate;
            }
            var diffDate = endDate.Subtract(now);
            var maxSeconds = Convert.ToInt32(diffDate.TotalSeconds);
            return maxSeconds;
        }
    }
}
