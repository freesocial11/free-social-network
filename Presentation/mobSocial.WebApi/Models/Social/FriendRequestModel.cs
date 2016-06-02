using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class FriendRequestsModel : RootModel
    {
        public FriendRequestsModel()
        {
            FriendRequests = new List<FriendRequestModel>();
        }

        public List<FriendRequestModel> FriendRequests { get; set; } 
        public CustomerNavigationModel NavigationModel { get; set; }
    }

    public class FriendRequestModel : RootModel
    {

        public int FriendId { get; set; }
        public string CustomerDisplayName { get; set; }
        public string CustomerLocation { get; set; }

        public string ProfileUrl { get; set; }

        public string ProfileThumbnailUrl { get; set; }

    }

}