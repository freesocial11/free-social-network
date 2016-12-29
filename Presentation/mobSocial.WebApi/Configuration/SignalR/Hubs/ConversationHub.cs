using System.Linq;
using System.Web.Http;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Conversations;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Controllers;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Conversation;
using Microsoft.AspNet.SignalR.Hubs;

namespace mobSocial.WebApi.Configuration.SignalR.Hubs
{
    [HubName("conversation")]
    public class ConversationHub : MobSocialHub
    {
        public IHttpActionResult PostReply(int userId, string replyText)
        {
            var conversationController = mobSocialEngine.ActiveEngine.Resolve<ConversationController>();
            return conversationController.Post(userId, new ConversationEntityModel() {
                Group = false,
                ReceiverId = userId,
                ReplyText = replyText
            });
        }

        public IHttpActionResult MarkRead(int conversationId)
        {
            var conversationController = mobSocialEngine.ActiveEngine.Resolve<ConversationController>();
            return conversationController.MarkRead(conversationId);
        }

        public void NotifyTyping(int conversationId, bool typing)
        {
            //we get conversation
            var conversationService = mobSocialEngine.ActiveEngine.Resolve<IConversationService>();
            var conversation = conversationService.Get(conversationId);
            if (conversation == null)
                return;
            var currentUser = ApplicationContext.Current.CurrentUser;
            var conversationUserIds = conversation.GetUserIds().Select(x => x.ToString()).ToList();
            conversationUserIds.Remove(currentUser.Id.ToString());//no need to notify the one who is typing
            Clients.Users(conversationUserIds).notifyTyping(conversationId, currentUser.Id, typing);
        }
    }
}