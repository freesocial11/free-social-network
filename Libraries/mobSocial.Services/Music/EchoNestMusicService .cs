using System;
using mobSocial.Core.oAuth;
using mobSocial.Data.Entity.Settings;

namespace mobSocial.Services.Music
{

    public class EchoNestMusicService : IMusicService
    {
        private readonly ThirdPartySettings _thirdPartySettings;
        
        public EchoNestMusicService(ThirdPartySettings thirdPartySettings) 
        {
            _thirdPartySettings = thirdPartySettings;
        }

        /// <summary>
        /// Gets the preview url for a sample of the song. 
        /// Jennifer Lopez - Jenny from the Block's track id is 19589723
        /// </summary>
        /// <param name="trackId">Track Id to get preview url for.</param>
        /// <returns></returns>
        public string GetTrackPreviewUrl(int trackId)
        {
            var url = "http://previews.7digital.com/clip/" + trackId.ToString() + "?country=US";
            var oauth = new OAuthBase();
            var previewUrl = oauth.GetSignedUrl(url, _thirdPartySettings.SevenDigitalOAuthConsumerKey, _thirdPartySettings.SevenDigitalOAuthConsumerSecret);
            return previewUrl;
        }

        public string GetTrackAffiliateUrl(int trackId)
        {
            var url = "https://instant.7digital.com/purchase/track/" + trackId.ToString() + "?partner=" + _thirdPartySettings.SevenDigitalPartnerId;
            return url;
        }

        public object SearchSongs(string term)
        {
            throw new NotImplementedException();
        }
    }

    

    



}
