using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Data.Entity.Conversations;
using mobSocial.Data.Entity.Settings;
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
            var userIds = conversation.GetUserIds();
            var replies = conversation.ConversationReplies.OrderByDescending(x => x.DateCreated).TakeFromPage(page, 15);
            var users = userService.Get(x => userIds.Contains(x.Id)).ToList().Select(x => x.ToModel(mediaService, mediaSettings));
            var model = new ConversationResponseModel()
            {
                ConversationId = conversation.Id,
                ConversationReplies = replies.Select(x => x.ToModel()).ToList(),
                Users = users.ToList()
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
                ReplyText = conversationReply.ReplyText
            };
            return model;
        }

        public static void AddUsers(this Conversation conversation, IEnumerable<int> userIds)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds");
            existingUserIds.AddRange(userIds);
            //set
            conversation.SetPropertyValue("UserIds", existingUserIds);
        }

        public static void RemoveUsers(this Conversation conversation, IEnumerable<int> userIds)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds");
            existingUserIds.RemoveAll(userIds.Contains);
            //set
            conversation.SetPropertyValue("UserIds", existingUserIds);
        }

        public static bool CanUserConverse(this Conversation conversation, int userId)
        {
            var existingUserIds = conversation.GetPropertyValueAs<List<int>>("UserIds");
            return existingUserIds.Contains(userId);
        }

        public static List<int> GetUserIds(this Conversation conversation)
        {
            return conversation.GetPropertyValueAs<List<int>>("UserIds");
        }
    }
}