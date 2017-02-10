using System;
using System.Web.Http;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Skills;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Battles;
using mobSocial.Services.Extensions;
using mobSocial.Services.Notifications;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("social")]
    public class FollowController : RootApiController
    {
        

        private readonly IFollowService _followService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public FollowController(IFollowService followService, INotificationService notificationService, IUserService userService)
        {
            _followService = followService;
            _notificationService = notificationService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        [Route("follow/{entityName}/{id:int}")]
        public IHttpActionResult Follow(string entityName, int id)
        {
            var response = false;
            var newStatus = 0;
            var currentUser = ApplicationContext.Current.CurrentUser;
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Follow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.User:
                    response = Follow<User>(id);
                    _notificationService.Notify(id, NotificationEventNames.UserFollowed, currentUser, "User", currentUser.Id, DateTime.UtcNow);
                    break;
                case FollowableEntityNames.Skill:
                    response = Follow<Skill>(id);
                    break;
            }
            if (response)
                newStatus = 1;
            return Json(new {Success = response, NewStatus = newStatus, NewStatusString = "Following"});
        }

        [HttpPost]
        [Authorize]
        [Route("unfollow/{entityName}/{id:int}")]
        public IHttpActionResult Unfollow(string entityName, int id)
        {
            var response = false;
            var newStatus = 1;
            var currentUser = ApplicationContext.Current.CurrentUser;
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Unfollow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.User:
                    response = Unfollow<User>(id);
                    _notificationService.DeNotify<User>(id, NotificationEventNames.UserFollowed, currentUser.Id, "User", currentUser.Id);
                    break;
                case FollowableEntityNames.Skill:
                    response = Unfollow<Skill>(id);
                    break;
            }
            if (response)
                newStatus = 0;
            return Json(new { Success = response, NewStatus = newStatus, NewStatusString = "Not Following" });
        }

        #region helpers
        private bool Follow<T>(int id)
        {

            _followService.Insert<T>(ApplicationContext.Current.CurrentUser.Id, id);
            return true;

        }

        private bool Unfollow<T>(int id)
        {
            _followService.Delete<T>(ApplicationContext.Current.CurrentUser.Id, id);
            return true;
        }

        #endregion

        #region inner classes

        private static class FollowableEntityNames
        {
            public const string VideoBattle = "videobattle";
            public const string User = "user";
            public const string Skill = "skill";
        }

        #endregion
    }
}