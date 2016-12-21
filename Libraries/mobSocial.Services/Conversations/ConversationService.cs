using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Conversations;

namespace mobSocial.Services.Conversations
{
    public class ConversationService : BaseEntityService<Conversation>, IConversationService
    {
        public ConversationService(IDataRepository<Conversation> dataRepository) : base(dataRepository)
        {
        }
    }
}