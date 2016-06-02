using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface ICommentService: IBaseEntityService<Comment>
    {
        int GetCommentsCount(int entityId, string entityName);

        IList<Comment> GetEntityComments(int entityId, string entityName, int page = 1, int count = 5);
    }
}