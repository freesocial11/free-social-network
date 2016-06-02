using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.Social;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("friends")]
    public class FriendsController : RootApiController
    {
        private readonly IFriendService _friendService;
        private readonly IMediaService _pictureService;
        private readonly IUserService _customerService;
        private readonly IFollowService _customerFollowService;
        private readonly IUserService _userService;

        public FriendsController(IFriendService friendService, IMediaService pictureService, IUserService customerService, IFollowService customerFollowService, IUserService userService)
        {
            _friendService = friendService;
            _pictureService = pictureService;
            _customerService = customerService;
            _customerFollowService = customerFollowService;
            _userService = userService;
        }

     

        [Authorize]
        [HttpPost]
        [Route("addfriend/{friendId:int}")]
        public IHttpActionResult AddFriend(int friendId)
        {

            if (ApplicationContext.Current.CurrentUser.Id == friendId)
                return Json(new { Success = false, Message = "Can't add one's self" });

            var fromCustomerId = ApplicationContext.Current.CurrentUser.Id;

            //first check if the request has already been sent?
            var customerFriend = _friendService.GetCustomerFriendship(fromCustomerId, friendId) ??
                                 new Friend() {
                                     FromCustomerId = fromCustomerId,
                                     ToCustomerId = friendId,
                                     DateRequested = DateTime.UtcNow
                                 };

            if (customerFriend.Confirmed)
            {
                return Json(new { Success = false, Message = "Already friends" });
            }

            if (customerFriend.Id == 0)
            {
                _friendService.Insert(customerFriend);

                //let's add customer follow
                _customerFollowService.Insert<User>(fromCustomerId, friendId);
            }


            return Json(new { Success = true, NewStatus = FriendStatus.FriendRequestSent });

        }

        [Authorize]
        [HttpPost]
        [Route("confirmfriend/{friendId:int}")]
        public IHttpActionResult ConfirmFriend(int friendId)
        {

            if (ApplicationContext.Current.CurrentUser.Id == friendId)
                return Json(new { Success = false, Message = "Can't add one's self" });

            var toCustomerId = ApplicationContext.Current.CurrentUser.Id;

            //first check if the request has already been sent?. Only the receiver can accept or decline
            var customerFriend = _friendService.GetCustomerFriend(friendId, toCustomerId);

            if (customerFriend == null)
                return Json(new { Success = false, Message = "No friendship request sent" });

            customerFriend.Confirmed = true;
            customerFriend.DateConfirmed = DateTime.UtcNow;
            _friendService.Update(customerFriend);

            return Json(new { Success = true, NewStatus = FriendStatus.Friends });
        }

        [Authorize]
        [HttpPost]
        [Route("declinefriend/{friendId:int}")]
        public IHttpActionResult DeclineFriend(int friendId)
        {

            if (ApplicationContext.Current.CurrentUser.Id == friendId)
                return Json(new { Success = false, Message = "Can't add one's self" });

            var Customer2Id = ApplicationContext.Current.CurrentUser.Id;

            //first check if the request has already been sent?. Any of two parties can decline
            var customerFriend = _friendService.GetCustomerFriendship(friendId, Customer2Id);

            if (customerFriend == null)
                return Json(new { Success = false, Message = "No friendship request sent" });

            _friendService.Delete(customerFriend);

            return Json(new { Success = true, NewStatus = FriendStatus.None });
        }

      
        [HttpGet]
        [Authorize]
        [Route("getfriendrequests")]
        public IHttpActionResult GetFriendRequests()
        {
            var friendRequests = _friendService.GetCustomerFriendRequests(ApplicationContext.Current.CurrentUser.Id);

            var friendRequestCustomers =
                _customerService.Get(x => friendRequests.Select(r => r.FromCustomerId).ToArray().Contains(x.Id), null);

            var model = new List<FriendPublicModel>();
            foreach (var c in friendRequestCustomers)
            {
                var friendModel = new FriendPublicModel() {
                    Id = c.Id,
                    DisplayName = c.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                    PictureUrl = _pictureService.GetPictureUrl(c.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId)),
                    ProfileUrl = Url.Route("CustomerProfileUrl",
                            new RouteValueDictionary() { {"SeName", c.GetPermalink().ToString() }}),
                    FriendStatus = FriendStatus.NeedsConfirmed
                };
                model.Add(friendModel);

            }

            return Json(new { Success = true, People = model });
        }

        [Authorize]
        [HttpGet]
        [Route("getcustomerfriends")]
        public IHttpActionResult GetCustomerFriends(int customerId , int howMany = 0, bool random = false)
        {

            if (customerId == 0)
                customerId = ApplicationContext.Current.CurrentUser.Id;

            var friends = _friendService.GetCustomerFriends(customerId);

            var model = new List<UserFriendModel>();

            foreach (var friend in friends)
            {

                var friendId = (friend.FromCustomerId == customerId) ? friend.ToCustomerId : friend.FromCustomerId;

                var friendCustomer = _customerService.Get(friendId);

                if (friendCustomer == null)
                    continue;

                var friendThumbnailUrl = _pictureService.GetPictureUrl(friendCustomer.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId));

                model.Add(new UserFriendModel() {
                    CustomerDisplayName = friendCustomer.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                    ProfileUrl =  Url.Route("CustomerProfileUrl",
                            new RouteValueDictionary()
                            {
                               { "SeName",  friendCustomer.GetPermalink().ToString() }  
                            }),
                    ProfileThumbnailUrl = friendThumbnailUrl
                });

            }

            return Json(new { Success = true, Friends = model });

        }
      

        [HttpGet]
        [Route("searchpeople")]
        public IHttpActionResult SearchPeople([FromUri] FriendSearchModel model)
        {

            var customers = _userService.SearchUsers(model.SearchTerm, model.ExcludeLoggedInUser, model.Page, model.Count);
            var models = new List<object>();

            var currentUser = ApplicationContext.Current.CurrentUser;
            //get all the friends of logged in customer
            IList<Friend> friends = null;
            if (currentUser.IsRegistered())
            {
                friends = _friendService.GetAllCustomerFriends(currentUser.Id);
            }

            if (friends == null)
                friends = new List<Friend>();

            foreach (var c in customers)
            {
                var friendModel = new FriendPublicModel() {
                    Id = c.Id,
                    DisplayName = c.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                    PictureUrl = _pictureService.GetPictureUrl(c.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId)),
                    ProfileUrl =
                        Url.Route("CustomerProfileUrl",
                            new RouteValueDictionary()
                            {
                               { "SeName",  c.GetPermalink().ToString() }  
                            }),
                };

                var friend = friends.FirstOrDefault(x => x.FromCustomerId == c.Id || x.ToCustomerId == c.Id);

                if (friend == null)
                    friendModel.FriendStatus = FriendStatus.None;
                else if (friend.Confirmed)
                    friendModel.FriendStatus = FriendStatus.Friends;
                else if (!friend.Confirmed && friend.FromCustomerId == currentUser.Id)
                    friendModel.FriendStatus = FriendStatus.FriendRequestSent;
                else if (currentUser.Id == c.Id)
                    friendModel.FriendStatus = FriendStatus.Self;
                else
                    friendModel.FriendStatus = FriendStatus.NeedsConfirmed;
                models.Add(friendModel);
            }
            return Json(new { Success = true, People = models });
        }

    }
}