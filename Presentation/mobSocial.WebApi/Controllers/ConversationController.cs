using System;
using System.Linq;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Data.Entity.Conversations;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Conversations;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.SignalR.Hubs;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Conversation;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("conversation")]
    [Authorize]
    public class ConversationController : RootApiWithHubController<ConversationHub>
    {
        private readonly IConversationService _conversationService;
        private readonly IConversationReplyService _conversationReplyService;
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;

        public ConversationController(IConversationService conversationService, IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, IConversationReplyService conversationReplyService)
        {
            _conversationService = conversationService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _conversationReplyService = conversationReplyService;
        }

        [Route("get")]
        public IHttpActionResult Get([FromUri] ConversationRequestModel requestModel)
        {
            //get the conversation
            var conversation = _conversationService.Get(requestModel.ConversationId);
            var model = conversation.ToModel(_userService, _mediaService, _mediaSettings, requestModel.Page);
            return RespondSuccess(new
            {
                Conversation = model
            });
        }

        [Route("post/{toUserId:int}")]
        [HttpPost]
        public IHttpActionResult Post(int toUserId, [FromBody] string replyText)
        {
            const string contextName = "conversation_post";
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (currentUser.Id == toUserId)
            {
                return RespondFailure("Can't have conversation with one's self", contextName);
            }

            //check if we have any previous conversation between logged in user and user in question
            var conversation =
                _conversationService.FirstOrDefault(
                    x =>
                        (x.UserId == currentUser.Id || x.UserId == toUserId) &&
                        x.ConversationReplies.Any(y => y.UserId == currentUser.Id || y.UserId == toUserId));


            if (conversation == null)
            {
                //we'll need to insert a new conversation
                conversation = new Conversation()
                {
                    CreatedOn = DateTime.UtcNow,
                    UserId = currentUser.Id
                };
                _conversationService.Insert(conversation);
            }

            //save the reply now
            var reply = new ConversationReply() {
                ConversationId = conversation.Id,
                ReplyText = replyText,
                DateCreated = DateTime.UtcNow,
                IpAddress = WebHelper.GetClientIpAddress(),
                UserId = currentUser.Id
            };
            _conversationReplyService.Insert(reply);

            //notify hubs
            var conversationUserIds = conversation.GetUserIds().Select(x => x.ToString()).ToList();
            Hub.Clients.Users(conversationUserIds).conversationReply(reply.ToModel());

            return RespondSuccess();
        }
    }
}