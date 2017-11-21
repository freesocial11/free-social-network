using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Data.Entity.Conversations;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Conversations;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.SignalR.Hubs;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Conversation;
using mobSocial.Data.Constants;
using mobSocial.WebApi.Configuration.OAuth;
using mobSocial.WebApi.Configuration.Security.Attributes;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("conversations")]
    [Authorize]
    public class ConversationController : RootApiWithHubController<ConversationHub>
    {
        private readonly IConversationService _conversationService;
        private readonly IConversationReplyService _conversationReplyService;
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IFriendService _friendService;

        public ConversationController(IConversationService conversationService, IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, IConversationReplyService conversationReplyService, IFriendService friendService)
        {
            _conversationService = conversationService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _conversationReplyService = conversationReplyService;
            _friendService = friendService;
        }

        [Route("get")]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsR)]
        public IHttpActionResult Get([FromUri] ConversationRequestModel requestModel)
        {
            //get the conversation
            var conversation = GetConversationWithUser(requestModel.UserId);
            var model = conversation?.ToModel(_userService, _mediaService, _mediaSettings, requestModel.Page);
            return RespondSuccess(new {
                Conversation = model
            });
        }
        [Route("get/all")]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsR)]
        public async Task<IHttpActionResult> Get()
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var conversations =
                await
                    _conversationService.Get(x => x.ConversationReplies.Any(y => x.UserId == currentUser.Id || x.ReceiverId == currentUser.Id),
                        x => new {x.LastUpdated}, false, earlyLoad: x => x.User).ToListAsync();

            //get all the users apart from current user
            var replies = await
                _conversationReplyService.Get(
                    x =>
                        x.ConversationReplyStatus.Any(
                            y => y.ReplyStatus == ReplyStatus.Received && y.UserId == currentUser.Id), earlyLoad: x => x.User).ToListAsync();

            var model = new List<ConversationOverviewModel>();
            foreach (var conversation in conversations)
            {
                var receiver = conversation.UserId == currentUser.Id
                    ? replies.FirstOrDefault()?.User : conversation.User;
                if (receiver == null)
                {
                    if(conversation.ReceiverType == "User")
                    {
                        receiver = _userService.Get(conversation.ReceiverId);
                        if (receiver == null)
                            continue;
                    }
                }
                var modelItem = new ConversationOverviewModel()
                {
                    ConversationId = conversation.Id,
                    UnreadCount = replies.Count(x => x.ConversationId == conversation.Id),
                    ReceiverName =  receiver.Name,
                    ReceiverId = receiver.Id,
                    ReceiverType = conversation.ReceiverType,
                    ReceiverImageUrl = _mediaService.GetPictureUrl(receiver.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId), PictureSizeNames.MediumProfileImage)
                };
                if (string.IsNullOrEmpty(modelItem.ReceiverImageUrl))
                    modelItem.ReceiverImageUrl = _mediaSettings.DefaultUserProfileImageUrl;
                model.Add(modelItem);
            }
            var allReceiverIds = model.Where(x => x.ReceiverType == "User").Select(x => x.ReceiverId).ToList();

            //more friends to conversation?
            var friendsIds = _friendService.GetFriends(currentUser.Id, 1, int.MaxValue)
                .Where(x => !allReceiverIds.Contains(x.FromCustomerId) && !allReceiverIds.Contains(x.ToCustomerId))
                .Select(x => x.FromCustomerId == currentUser.Id ? x.ToCustomerId : x.FromCustomerId).ToList();

            var userModels = _userService.Get(x => friendsIds.Contains(x.Id)).ToList();

            foreach (var uModel in userModels)
            {
                var modelItem = new ConversationOverviewModel()
                {
                    ConversationId = 0,
                    UnreadCount = 0,
                    ReceiverName = uModel.Name,
                    ReceiverId = uModel.Id,
                    ReceiverType = "User",
                    ReceiverImageUrl = _mediaService.GetPictureUrl(uModel.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId), PictureSizeNames.MediumProfileImage)
                };
                if (string.IsNullOrEmpty(modelItem.ReceiverImageUrl))
                    modelItem.ReceiverImageUrl = _mediaSettings.DefaultUserProfileImageUrl;
                model.Add(modelItem);
            }

            return RespondSuccess(new
            {
                Conversations = model
            });
        }

        [Route("reply/read/put")]
        [HttpPut]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsRW)]
        public IHttpActionResult MarkRead(int[] replyIds)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            //get replies
            var replies = _conversationReplyService.Get(x => replyIds.Contains(x.Id), earlyLoad: x => x.ConversationReplyStatus);
            //update all replies
            foreach (var reply in replies)
            {
                var status = reply.ConversationReplyStatus.FirstOrDefault(
                    x => x.UserId == currentUser.Id && x.ReplyStatus != ReplyStatus.Deleted && x.ReplyStatus != ReplyStatus.Read);
                if (status == null)
                    continue;
                status.ReplyStatus = ReplyStatus.Read;
                //update conversation reply
                _conversationReplyService.Update(reply);
            }
            return RespondSuccess();
        }

        [Route("read/put")]
        [HttpPut]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsRW)]
        public IHttpActionResult MarkRead(int conversationId)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            //get replies
            var replies = _conversationReplyService.Get(x => x.ConversationId == conversationId, earlyLoad: x => x.ConversationReplyStatus).ToList();
            //update all replies
            foreach (var reply in replies)
            {
                var status = reply.ConversationReplyStatus.FirstOrDefault(
                    x => x.UserId == currentUser.Id && x.ReplyStatus != ReplyStatus.Deleted && x.ReplyStatus != ReplyStatus.Read);
                if (status == null)
                    continue;
                status.ReplyStatus = ReplyStatus.Read;
                //update conversation reply
                _conversationReplyService.Update(reply);
            }
            var conversation = replies.FirstOrDefault()?.Conversation ?? _conversationService.Get(conversationId);
            //update last updated
            conversation.LastUpdated = DateTime.UtcNow;
            _conversationService.Update(conversation);
            return RespondSuccess();
        }

        [Route("reply/delete")]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsRWD)]
        public IHttpActionResult DeleteReply(int replyId)
        {
            //let's query the reply first
            var reply = _conversationReplyService.Get(replyId, earlyLoad: x => x.Conversation);
            var currentUser = ApplicationContext.Current.CurrentUser;

            //is current user actually part of this conversation to delete this reply
            if(!reply.Conversation.CanUserConverse(currentUser.Id))
                return Unauthorized();

            var replyStatus = reply.ConversationReplyStatus.First(x => x.UserId == currentUser.Id);
            replyStatus.ReplyStatus = ReplyStatus.Deleted;
            _conversationReplyService.Update(reply);
            return RespondSuccess();
        }

        [Route("post/{toUserId:int}")]
        [HttpPost]
        [ScopeAuthorize(Scope = OAuthScopes.ConversationsRW)]
        public IHttpActionResult Post(int toUserId, ConversationEntityModel requestModel)
        {
            const string contextName = "conversation_post";
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (currentUser == null)
                throw new Exception("user not found");
            if (currentUser.Id == toUserId)
            {
                return RespondFailure("Can't have conversation with one's self", contextName);
            }

            //check if we have any previous conversation between logged in user and user in question
            var conversation = GetConversationWithUser(toUserId, true);

            if (conversation == null)
            {
                //we'll need to insert a new conversation
                conversation = new Conversation() {
                    CreatedOn = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    UserId = currentUser.Id,
                    ReceiverType = !requestModel.Group ? typeof(User).Name : "Group",
                    ReceiverId = toUserId
                };
                _conversationService.Insert(conversation);

                //set user ids to allow conversation
                conversation.AddUsers(new List<int>() { currentUser.Id, toUserId });
            }

            //save the reply now
            var reply = new ConversationReply() {
                ConversationId = conversation.Id,
                ReplyText = requestModel.ReplyText,
                DateCreated = DateTime.UtcNow,
                IpAddress = WebHelper.GetClientIpAddress(),
                UserId = currentUser.Id,
                ConversationReplyStatus = new List<ConversationReplyStatus>()
            };

            //add each conversation user with their reply status
            var conversationUsers = conversation.GetUserIds();
            foreach (var userId in conversationUsers)
            {
                var replyStatus = userId == currentUser.Id ? ReplyStatus.Sent : ReplyStatus.Received;
                reply.ConversationReplyStatus.Add(new ConversationReplyStatus()
                {
                    UserId = userId,
                    ReplyStatus = replyStatus,
                    LastUpdated = DateTime.UtcNow,
                    Reply = reply
                });
            }
            _conversationReplyService.Insert(reply);

            var model = reply.ToModel();
            Hub.Clients.User(currentUser.Id.ToString()).conversationReply(model, conversation.Id, toUserId);

            //change reply status for other receivers
            model.ReplyStatus = 0;
            //notify hubs except current user
            var conversationUserIds = conversation.GetUserIds().Select(x => x.ToString()).ToList();
            conversationUserIds.Remove(currentUser.Id.ToString());
            Hub.Clients.Users(conversationUserIds).conversationReply(model, conversation.Id);

            return RespondSuccess();
        }

        private Conversation GetConversationWithUser(int userId, bool includeDeleted = false)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var receiverType = typeof(User).Name;
            var conversation =
                _conversationService.FirstOrDefault(
                    x =>
                        x.ReceiverType == receiverType &&
                        ((x.UserId == currentUser.Id && x.ReceiverId == userId) ||
                         (x.UserId == userId && x.ReceiverId == currentUser.Id)),
                    earlyLoad:
                    x =>
                        x.ConversationReplies.Select(y => y.ConversationReplyStatus));

            if (!includeDeleted && conversation != null)
                //exclude replies which user assumes deleted
                conversation.ConversationReplies =
                    conversation.ConversationReplies.Where(
                        x =>
                            x.ConversationReplyStatus.Any(
                                y => y.UserId == currentUser.Id && y.ReplyStatus != ReplyStatus.Deleted)).ToList();
            return conversation;
        }
    }
}