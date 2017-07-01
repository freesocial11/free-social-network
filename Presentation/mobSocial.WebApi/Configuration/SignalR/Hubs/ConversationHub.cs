using System.Linq;
using System.Web.Http;
using DryIoc;
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
            using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
            {
                var conversationController = mobSocialEngine.ActiveEngine.Resolve<ConversationController>();
                return conversationController.Post(userId, new ConversationEntityModel()
                {
                    Group = false,
                    ReceiverId = userId,
                    ReplyText = replyText
                });
            }
           
        }

        public void MarkRead(int conversationId)
        {
            using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
            {
                var conversationController = mobSocialEngine.ActiveEngine.Resolve<ConversationController>();
                conversationController.MarkRead(conversationId);

                //we get conversation
                var conversationService = mobSocialEngine.ActiveEngine.Resolve<IConversationService>();
                var conversation = conversationService.Get(conversationId);
                if (conversation == null)
                    return;
                var currentUser = ApplicationContext.Current.CurrentUser;
                var conversationUserIds = conversation.GetUserIds().Select(x => x.ToString()).ToList();
                conversationUserIds.Remove(currentUser.Id.ToString());//no need to notify the one who is reading
                Clients.Users(conversationUserIds).markRead(conversationId);
            }

            
        }

        public void NotifyTyping(int conversationId, bool typing)
        {
            using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
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
}