using System.Collections.Generic;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Notifications
{
    /// <summary>
    /// Social Network Message service. Used for sending social network notifications.
    /// </summary>
    public interface IMobSocialMessageService
    {
        int SendFriendRequestNotification(User user, int friendRequestCount, int languageId, int storeId);

        int SendEventInvitationNotification(User user, int languageId, int storeId);

        int SendPendingFriendRequestNotification(User user, int friendRequestCount, int languageId, int storeId);

        int SendBirthdayNotification(User user, int languageId, int storeId);

        int SendSomeoneSentYouASongNotification(User user, int languageId, int storeId);

        int SendSomeoneChallengedYouForABattleNotification(User challenger, User challengee, VideoBattle videoBattle, int languageId, int storeId);

        int SendSomeoneChallengedYouForABattleNotification(User challenger, string challengeeEmail, string challengeeName, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleCompleteNotification(User user, VideoBattle videoBattle, NotificationRecipientType recipientType, int languageId, int storeId);

        int SendVotingReminderNotification(User sender, User receiver, VideoBattle videoBattle, int languageId, int storeId);

        int SendVotingReminderNotification(User sender, string receiverEmail, string receiverName, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleSignupNotification(User challenger, User challengee, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleJoinNotification(User challenger, User challengee, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleSignupAcceptedNotification(User challenger, User challengee, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleDisqualifiedNotification(User challenger, User challengee, VideoBattle videoBattle, int languageId, int storeId);

        int SendVideoBattleOpenNotification(User receiver, VideoBattle videoBattle, int languageId, int storeId);

        int SendSponsorAppliedNotificationToBattleOwner(User owner, User sponsor, VideoBattle videoBattle, int languageId, int storeId);

        int SendSponsorshipStatusChangeNotification(User receiver, SponsorshipStatus sponsorshipStatus, VideoBattle videoBattle, int languageId, int storeId);

        int SendXDaysToBattleStartNotificationToParticipant(User receiver, VideoBattle videoBattle, int languageId, int storeId);

        int SendXDaysToBattleEndNotificationToFollower(User receiver, VideoBattle videoBattle, int languageId, int storeId);

    }

}
