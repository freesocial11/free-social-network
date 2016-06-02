using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Songs;

namespace mobSocial.Services.Songs
{
    public interface ISongService : IBaseEntityService<Song>
    {
        IList<Song> SearchSongs(string term, int count = 15, int page = 1, bool searchDescriptions = false, bool searchArtists = false, string artistName = "", bool publishedOnly = true);

        IList<Song> SearchSongs(string term, out int totalPages, int count = 15, int page = 1, bool searchDescriptions = false, bool searchArtists = false, string artistName = "", bool publishedOnly = true);

        Song GetSongByRemoteEntityId(string remoteEntityId);

        Song GetSongByProductId(int productId);
    }
}
