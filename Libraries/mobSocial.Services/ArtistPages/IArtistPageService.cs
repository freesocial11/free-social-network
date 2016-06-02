using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.ArtistPages;

namespace mobSocial.Services.ArtistPages
{
    public interface IArtistPageService : IBaseEntityService<ArtistPage>
    {
        ArtistPage GetArtistPageByName(string name);

        IList<ArtistPage> GetArtistPagesByPageOwner(int pageOwnerId, string searchTerm = "", int count = 15, int page = 1, bool includeOrphan = false);
        IList<ArtistPage> GetArtistPagesByPageOwner(int pageOwnerId, out int totalPages, string searchTerm = "", int count = 15, int page = 1, bool includeOrphan = false);


        IList<ArtistPage> GetArtistPagesByRemoteEntityId(string[] remoteEntityId);

        IList<ArtistPage> SearchArtists(string term, int count = 15, int page = 1, bool searchDescriptions = false);

        IList<ArtistPage> SearchArtists(string term, out int totalPages, int count = 15, int page = 1, bool searchDescriptions = false);

    }
}
