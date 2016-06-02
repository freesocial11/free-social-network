namespace mobSocial.Services.Music
{
    /// <summary>
    /// Music service for searching artists, tracks, and getting track previews
    /// </summary>
    public interface IMusicService
    {
        string GetTrackPreviewUrl(int trackId);

        // just place holders that can be refactored later
        string GetTrackAffiliateUrl(int trackId);
        object SearchSongs(string term);

    }

}
