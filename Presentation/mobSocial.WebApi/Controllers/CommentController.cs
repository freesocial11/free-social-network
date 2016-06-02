using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.Social;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("comments")]
    public class CommentController : RootApiController
    {
        private readonly ICommentService _customerCommentService;
        private readonly ILikeService _customerLikeService;
        private readonly IUserService _customerService;
        private readonly IMediaService _pictureService;
        private readonly MediaSettings _mediaSettings;

        public CommentController(ICommentService customerCommentService,
            IUserService customerService,
            ILikeService customerLikeService,
            IMediaService pictureService, MediaSettings mediaSettings)
        {
            _customerCommentService = customerCommentService;
            _customerService = customerService;
            _customerLikeService = customerLikeService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        [Route("post")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post(UserCommentModel model)
        {
            if (!ModelState.IsValid)
                return Response(new { Success = false });

            //save the comment
            var comment = new Comment() {
                AdditionalData = model.AdditionalData,
                CommentText = model.CommentText,
                EntityName = model.EntityName,
                EntityId = model.EntityId,
                DateCreated = DateTime.UtcNow,
                UserId = ApplicationContext.Current.CurrentUser.Id
            };
            _customerCommentService.Insert(comment);
            var cModel = PrepareCommentPublicModel(comment, new[] { ApplicationContext.Current.CurrentUser });
            return Response(new { Success = true, Comment = cModel });
        }

        [Route("get")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult Get([FromUri] UserCommentRequestModel model)
        {
            if (!ModelState.IsValid)
                return Response(new { Success = false });
            if (model.Page <= 0)
                model.Page = 1;
            if (model.Count <= 0)
                model.Count = 5;

            //retrieve the comments
            var comments = _customerCommentService.GetEntityComments(model.EntityId, model.EntityName, model.Page, model.Count);
            var commentModels = new List<UserCommentPublicModel>();

            //retrieve all the associated customers at ones for performance reasons
            var customers = _customerService.Get(x => comments.Select(y => y.UserId).Contains(x.Id), null);

            foreach (var comment in comments)
            {
                var cModel = PrepareCommentPublicModel(comment, customers);
                commentModels.Add(cModel);
            }

            //send the response
            return Response(new { Success = true, Comments = commentModels });
        }

        [Route("delete/{commentId:int}")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult Delete(int commentId)
        {
            //only administrator or comment owner can delete the comment, so first let's retrieve the comment
            var comment = _customerCommentService.Get(commentId);
            if (comment == null)
                return Response(new { Success = false, Message = "Comment doesn't exist" });
            //so who is ringing the bell?
            if (comment.UserId != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator())
                return Response(new { Success = false, Message = "Unauthorized" });

            //come in and delete the comment
            _customerCommentService.Delete(comment);

            return Response(new { Success = true });
        }


        #region helpers

        private UserCommentPublicModel PrepareCommentPublicModel(Comment comment, IEnumerable<User> users)
        {
            //get the customer
            var user = users.FirstOrDefault(x => x.Id == comment.UserId);
            if (user == null)
                return null;
            var likeStatus = _customerLikeService.GetCustomerLike<Comment>(ApplicationContext.Current.CurrentUser.Id, comment.Id) == null ? 0 : 1;
            //and create it's response model
            var cModel = new UserCommentPublicModel() {
                EntityName = comment.EntityName,
                EntityId = comment.EntityId,
                CommentText = comment.CommentText,
                AdditionalData = comment.AdditionalData,
                Id = comment.Id,
                DateCreatedUtc = comment.DateCreated,
                DateCreated =DateTimeHelper.GetDateInUserTimeZone(comment.DateCreated, DateTimeKind.Utc, user),
                CanDelete = comment.UserId == ApplicationContext.Current.CurrentUser.Id || ApplicationContext.Current.CurrentUser.IsAdministrator(),
                IsSpam = false, //TODO: change it when spam system has been implemented
                LikeCount = _customerLikeService.GetLikeCount<Comment>(comment.Id),
                UserName = user.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                UserProfileUrl = Url.Route("CustomerProfileUrl", new RouteValueDictionary()
                    {
                        {"SeName", user.GetPermalink().ToString() }
                    }),
                UserProfileImageUrl = _pictureService.GetPictureUrl(user.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId), PictureSizeNames.SmallProfileImage),
                LikeStatus = likeStatus
            };
            return cModel;
        }
        #endregion
    }
}
