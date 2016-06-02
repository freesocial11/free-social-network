using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.ArtistPages;

namespace mobSocial.Services.ArtistPages
{
    public interface IArtistPageManagerService : IBaseEntityService<ArtistPageManager>
    {
        void AddPageManager(ArtistPageManager manager);

        void DeletePageManager(ArtistPageManager manager);

        void DeletePageManager(int artistPageId, int customerId);

        bool IsPageManager(int artistPageId, int customerId);

        IList<ArtistPageManager> GetPageManagers(int artistPageId);

        IList<ArtistPage> GetPagesAsManager(int customerId);
    }
}
