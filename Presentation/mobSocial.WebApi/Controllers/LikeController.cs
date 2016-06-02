using System.Web.Http;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Timeline;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Social;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("like")]
    public class LikeController : RootApiController
    {
        

        private readonly ILikeService _customerLikeService;

        public LikeController(ILikeService customerLikeService)
        {
            _customerLikeService = customerLikeService;
        }

        [HttpPost]
        [Authorize]
        [Route("like/{entityName}/{id:int}")]
        public IHttpActionResult Like(string entityName, int id)
        {
            var response = false;
            var newStatus = 0;
            switch (entityName.ToLower())
            {
                case LikableEntityNames.VideoBattle:
                    response = Like<VideoBattle>(id);
                    break;
                case LikableEntityNames.User:
                    response = Like<User>(id);
                    break;
                case LikableEntityNames.TimelinePost:
                    response = Like<TimelinePost>(id);
                    break;
                case LikableEntityNames.Comment:
                    response = Like<Comment>(id);
                    break;
            }
            if (response)
                newStatus = 1;
            return Json(new {Success = response, NewStatus = newStatus, NewStatusString = "Liked"});
        }

        [HttpPost]
        [Authorize]
        [Route("unlike/{entityName}/{id:int}")]
        public IHttpActionResult Unlike(string entityName, int id)
        {
            var response = false;
            var newStatus = 1;
            switch (entityName.ToLower())
            {
                case LikableEntityNames.VideoBattle:
                    response = Unlike<VideoBattle>(id);
                    break;
                case LikableEntityNames.User:
                    response = Unlike<User>(id);
                    break;
                case LikableEntityNames.TimelinePost:
                    response = Unlike<TimelinePost>(id);
                    break;
                case LikableEntityNames.Comment:
                    response = Unlike<Comment>(id);
                    break;
            }
            if (response)
                newStatus = 0;
            return Json(new { Success = response, NewStatus = newStatus, NewStatusString = "Unliked" });
        }

        #region helpers
        private bool Like<T>(int id)
        {

            _customerLikeService.Insert<T>(ApplicationContext.Current.CurrentUser.Id, id);
            return true;

        }

        private bool Unlike<T>(int id)
        {
            _customerLikeService.Delete<T>(ApplicationContext.Current.CurrentUser.Id, id);
            return true;
        }

        #endregion

        #region inner classes

        private static class LikableEntityNames
        {
            public const string VideoBattle = "videobattle";
            public const string User = "user";
            public const string TimelinePost = "timelinepost";
            public const string Comment = "comment";
        }

        #endregion
    }
}