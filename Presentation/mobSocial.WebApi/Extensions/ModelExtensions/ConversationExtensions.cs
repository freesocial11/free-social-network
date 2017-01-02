using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Data.Entity.Conversations;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.Data.Extensions;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.Conversation;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class ConversationExtensions
    {
        public static ConversationResponseModel ToModel(this Conversation conversation, IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, int page)
        {
            var userIds = conversation.GetUserIds().ToArray();
            var replyCount = 15;
            var totalReplies = conversation.ConversationReplies.Count;
            var replies = conversation.ConversationReplies?.OrderBy(x => x.DateCreated).TakeLast(replyCount, (page - 1) * replyCount);
            var users =
                userService.Get(x => userIds.Contains(x.Id))
                    .ToList()
                    .Select(x => x.ToModel(mediaService, mediaSettings))
                    .ToDictionary(x => x.Id, x => x);
            var model = new ConversationResponseModel()
            {
                ConversationId = conversation.Id,
                ConversationReplies = replies?.Select(x => x.ToModel()).ToList(),
                Users = users,
                ReceiverId = conversation.ReceiverId,
                ReceiverType = conversation.ReceiverType,
                UserId = conversation.UserId,
                TotalReplies = totalReplies
            };
            return model;
        }

        public static ConversationReplyModel ToModel(this ConversationReply conversationReply)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var model = new ConversationReplyModel()
            {
                UserId = conversationReply.UserId,
                DateCreatedUtc = conversationReply.DateCreated,
                DateCreatedLocal =
                    DateTimeHelper.GetDateInUserTimeZone(conversationReply.DateCreated, DateTimeKind.Utc, currentUser),
                ReplyText = conversationReply.ReplyText,
                ConversationId = conversationReply.ConversationId
            };

            if (currentUser.Id == conversationReply.UserId)
            {
                var replyStatus = conversationReply.ConversationReplyStatus.FirstOrDefault(x => x.UserId != currentUser.Id);
                //it's the reply of logged in user, so we'll send if the message has been read or not by the receiver
                if (replyStatus != null)
                {
                    model.ReplyStatus = replyStatus.ReplyStatus;
                    model.ReplyStatusLastUpdated = replyStatus.LastUpdated;
                    if (model.ReplyStatus == ReplyStatus.Deleted)
                    {
                        //we don't want to let other person know of this
                        model.ReplyStatus = ReplyStatus.Read;
                    }
                }
            }
            return model;
        }

        public static void AddUsers(this Conversation conversation, IEnumerable<int> userIds)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds", new List<int>());
            existingUserIds.AddRange(userIds);
            //set
            conversation.SetPropertyValue("UserIds", existingUserIds);
        }

        public static void RemoveUsers(this Conversation conversation, IEnumerable<int> userIds)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds", new List<int>());
            existingUserIds.RemoveAll(userIds.Contains);
            //set
            conversation.SetPropertyValue("UserIds", existingUserIds);
        }

        public static bool CanUserConverse(this Conversation conversation, int userId)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds", new List<int>());
            return existingUserIds.Contains(userId);
        }

        public static List<int> GetUserIds(this Conversation conversation)
        {
            return conversation.GetPropertyValueAs<List<int>>("UserIds");
        }
    }
}