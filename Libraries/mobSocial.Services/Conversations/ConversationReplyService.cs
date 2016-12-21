using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Conversations;

namespace mobSocial.Services.Conversations
{
    public class ConversationReplyService : BaseEntityService<ConversationReply>, IConversationReplyService
    {
        public ConversationReplyService(IDataRepository<ConversationReply> dataRepository) : base(dataRepository)
        {
        }
    }
}