using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationOverviewModel : RootModel
    {
        public int ConversationId { get; set; }

        public UserResponseModel Receiver { get; set; }

        public int UnreadCount { get; set; }
    }
}