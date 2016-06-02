using System.Web.Http;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Social;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("follow")]
    public class FollowController : RootApiController
    {
        

        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost]
        [Authorize]
        [Route("follow/{entityName}/{id:int}")]
        public IHttpActionResult Follow(string entityName, int id)
        {
            var response = false;
            var newStatus = 0;
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Follow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.User:
                    response = Follow<User>(id);
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
            switch (entityName.ToLower())
            {
                case FollowableEntityNames.VideoBattle:
                    response = Unfollow<VideoBattle>(id);
                    break;
                case FollowableEntityNames.User:
                    response = Unfollow<User>(id);
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
        }

        #endregion
    }
}