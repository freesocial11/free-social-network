using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class ThirdPartySettings : ISettingGroup
    {
        /// <summary>
        /// The Api Key for echonest
        /// </summary>
        public string EchonestApiKey { get; set; }

        /// <summary>
        /// 7Digital partner id
        /// </summary>
        public string SevenDigitalPartnerId { get; set; }

        /// <summary>
        /// 7Digital oAuth consumer key
        /// </summary>
        public string SevenDigitalOAuthConsumerKey { get; set; }

        /// <summary>
        /// 7Digital oAuth consumer secret
        /// </summary>
        public string SevenDigitalOAuthConsumerSecret { get; set; }

    }
}