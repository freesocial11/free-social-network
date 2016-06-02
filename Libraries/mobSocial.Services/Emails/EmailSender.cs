using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Emails
{
    public class EmailSender : IEmailSender
    {
        public int SendFriendRequestNotification(User user, int friendRequestCount)
        {
            throw new System.NotImplementedException();
        }

        public int SendEventInvitationNotification(User user)
        {
            throw new System.NotImplementedException();
        }

        public int SendPendingFriendRequestNotification(User user, int friendRequestCount)
        {
            throw new System.NotImplementedException();
        }

        public int SendBirthdayNotification(User user)
        {
            throw new System.NotImplementedException();
        }

        public int SendSomeoneSentYouASongNotification(User userUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendSomeoneChallengedYouForABattleNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendSomeoneChallengedYouForABattleNotification(User challenger, string challengeeEmail, string challengeeName,
            VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleCompleteNotification(User user, VideoBattle videoBattle, NotificationRecipientType recipientTypeUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVotingReminderNotification(User sender, User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVotingReminderNotification(User sender, string receiverEmail, string receiverName, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleSignupNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleJoinNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleSignupAcceptedNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleDisqualifiedNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendVideoBattleOpenNotification(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendSponsorAppliedNotificationToBattleOwner(User owner, User sponsor, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendSponsorshipStatusChangeNotification(User receiver, SponsorshipStatus sponsorshipStatus,
            VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendXDaysToBattleStartNotificationToParticipant(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public int SendXDaysToBattleEndNotificationToFollower(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }
    }
}