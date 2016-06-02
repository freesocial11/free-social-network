
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public class CommentService : MobSocialEntityService<Comment>, ICommentService
    {
        public CommentService(IDataRepository<Comment> repository)
            : base(repository)
        {
        }
        public int GetCommentsCount(int entityId, string entityName)
        {
            return
                Repository.Count(x => x.EntityId == entityId && x.EntityName == entityName);
        }

        public IList<Comment> GetEntityComments(int entityId, string entityName, int page = 1, int count = 5)
        {
            return
                Repository.Get(x => x.EntityId == entityId && x.EntityName == entityName)
                    .OrderBy(x => x.DateCreated)
                    .Skip(count*(page - 1))
                    .Take(count)
                    .ToList();
        }
    }
}
