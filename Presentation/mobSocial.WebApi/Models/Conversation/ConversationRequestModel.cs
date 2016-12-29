using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationRequestModel : RootModel
    {
        public int UserId { get; set; }

        public int Page { get; set; }
    }
}