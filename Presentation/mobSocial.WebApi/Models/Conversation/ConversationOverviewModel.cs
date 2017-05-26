using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationOverviewModel : RootModel
    {
        public int ConversationId { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverType { get; set; }

        public string ReceiverImageUrl { get; set; }

        public int UnreadCount { get; set; }
    }
}