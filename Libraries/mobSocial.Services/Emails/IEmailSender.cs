using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Emails;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Emails
{
    public interface IEmailSender
    {
        bool SendTestEmail(string email, EmailAccount emailAccount);

        void SendUserRegisteredMessage(User user, bool withAdmin = true);

        void SendUserActivationLinkMessage(User user, string activationUrl);

        void SendUserActivatedMessage(User user);

        void SendFriendRequestNotification(User user, int friendRequestCount);

        void SendEventInvitationNotification(User user);

        void SendPendingFriendRequestNotification(User user, int friendRequestCount);

        void SendBirthdayNotification(User user);

        void SendSomeoneSentYouASongNotification(User userUser);

        void SendSomeoneChallengedYouForABattleNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        void SendSomeoneChallengedYouForABattleNotification(User challenger, string challengeeEmail, string challengeeName, VideoBattle videoBattleUser);

        void SendVideoBattleCompleteNotification(User user, VideoBattle videoBattle, NotificationRecipientType recipientTypeUser);

        void SendVotingReminderNotification(User sender, User receiver, VideoBattle videoBattleUser);

        void SendVotingReminderNotification(User sender, string receiverEmail, string receiverName, VideoBattle videoBattleUser);

        void SendVideoBattleSignupNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        void SendVideoBattleJoinNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        void SendVideoBattleSignupAcceptedNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        void SendVideoBattleDisqualifiedNotification(User challenger, User challengee, VideoBattle videoBattleUser);

        void SendVideoBattleOpenNotification(User receiver, VideoBattle videoBattleUser);

        void SendSponsorAppliedNotificationToBattleOwner(User owner, User sponsor, VideoBattle videoBattleUser);

        void SendSponsorshipStatusChangeNotification(User receiver, SponsorshipStatus sponsorshipStatus, VideoBattle videoBattleUser);

        void SendXDaysToBattleStartNotificationToParticipant(User receiver, VideoBattle videoBattleUser);

        void SendXDaysToBattleEndNotificationToFollower(User receiver, VideoBattle videoBattleUser);
    }
}