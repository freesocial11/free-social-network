using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Emails
{
    public interface IEmailSender
    {
        int SendFriendRequestNotification(User user, int friendRequestCount);

        int SendEventInvitationNotification(User user);

        int SendPendingFriendRequestNotification(User user, int friendRequestCount);

        int SendBirthdayNotification(User user);

        int SendSomeoneSentYouASongNotification(User userUser);

        int SendSomeoneChallengedYouForABattleNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        int SendSomeoneChallengedYouForABattleNotification(User challenger, string challengeeEmail, string challengeeName, VideoBattle videoBattleUser);

        int SendVideoBattleCompleteNotification(User user, VideoBattle videoBattle, NotificationRecipientType recipientTypeUser);

        int SendVotingReminderNotification(User sender, User receiver, VideoBattle videoBattleUser);

        int SendVotingReminderNotification(User sender, string receiverEmail, string receiverName, VideoBattle videoBattleUser);

        int SendVideoBattleSignupNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        int SendVideoBattleJoinNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        int SendVideoBattleSignupAcceptedNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        int SendVideoBattleDisqualifiedNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        int SendVideoBattleOpenNotification(User receiver, VideoBattle videoBattleUser);

        int SendSponsorAppliedNotificationToBattleOwner(User owner, User sponsor, VideoBattle videoBattleUser);

        int SendSponsorshipStatusChangeNotification(User receiver, SponsorshipStatus sponsorshipStatus, VideoBattle videoBattleUser);

        int SendXDaysToBattleStartNotificationToParticipant(User receiver, VideoBattle videoBattleUser);

        int SendXDaysToBattleEndNotificationToFollower(User receiver, VideoBattle videoBattleUser);
    }
}