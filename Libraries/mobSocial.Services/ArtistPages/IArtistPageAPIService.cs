using System.Collections.Generic;

namespace mobSocial.Services.ArtistPages
{
    public interface IArtistPageApiService
    {
        bool DoesRemoteArtistExist(string name);

        string GetRemoteArtist(string name);

        IList<string> GetRelatedArtists(string remoteEntityId, int count = 5);

        IList<string> SearchArtists(string term, int count = 15, int page = 1);

        IList<string> GetArtistSongs(string artistName, int count = 15, int page = 1);

        string GetRemoteSong(string remoteEntityId);

        IList<string> SearchSongs(string term, string artist = "", int count = 15, int page = 1);

        IList<string> GetSimilarSongs(string trackId, int count = 5);
    }
}
