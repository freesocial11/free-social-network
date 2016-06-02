using System.Collections.Generic;
using System.Linq;
using System.Text;
using mobSocial.Core.Caching;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mobSocial.Services.ArtistPages
{
    public class ArtistPageApiService: IArtistPageApiService
    {
        private readonly ThirdPartySettings _thirdPartySettings;
        private readonly ICacheProvider _cacheProvider;

        public ArtistPageApiService(ThirdPartySettings thirdPartySettings, ICacheProvider cacheProvider)
        {
            _thirdPartySettings = thirdPartySettings;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Returns the API url. 
        /// </summary>
        /// <param name="endPoint">The API endpoint e.g. artist/similar </param>
        /// <param name="parameterString">Any additional parameter string to be passed to the api</param>
        /// <returns>The formatted URI for api call</returns>
        string GetApiUrl(string endPoint, string parameterString = "")
        {

            string urlFormat = "http://developer.echonest.com/api/v4/{0}?api_key={1}&format=json&{2}";
            return string.Format(urlFormat, endPoint, _thirdPartySettings.EchonestApiKey , parameterString);

        }

        public bool DoesRemoteArtistExist(string name)
        {
            //http://developer.echonest.com/docs/v4/artist.html#search
            var apiUrl = GetApiUrl("artist/search", string.Format("name={0}&results=1", name));
            var responseBytes = HttpHelper.ExecuteGet(apiUrl);

            if (responseBytes == null || responseBytes.Length == 0)
                return false;

            var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));

            return jsonObject["response"]["artists"].Any();

        }

        /// <summary>
        /// Returns remote artist as JSON string
        /// </summary>
        /// <param name="name">The name of artist</param>
        public string GetRemoteArtist(string name)
        {
            //http://developer.echonest.com/docs/v4/artist.html#profile
            var apiUrl = GetApiUrl("artist/profile", string.Format("name={0}&bucket=biographies&bucket=artist_location&bucket=images&bucket=songs&bucket=years_active", name));

            var responseBytes = HttpHelper.ExecuteGet(apiUrl);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));

            //create a generic object so that we can replace the api easily if required

            var artist = jsonObject["response"]["artist"];
            return ParseEchonestArtist(artist);           
        }

        /// <summary>
        /// Gets related artists based on current artists
        /// </summary>
        /// <param name="remoteEntityId">The entity id of artist on remote server. We store it in Artist.RemoteEntityId</param>
        /// <param name="count">Maximum number of results to be returned</param>
        public IList<string> GetRelatedArtists(string remoteEntityId, int count = 5)
        {
            //http://developer.echonest.com/docs/v4/artist.html#similar
            var apiUrl = GetApiUrl("artist/similar", string.Format("name={0}&bucket=biographies&bucket=artist_location&bucket=images&bucket=songs&bucket=years_active&results={1}", remoteEntityId, count));

            var responseBytes = HttpHelper.ExecuteGet(apiUrl);
            if (responseBytes== null || responseBytes.Length == 0)
                return null;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));

            var relatedArtists = new List<string>();
            for (int index = 0; index < jsonObject["response"]["artists"].Count(); index++)
            {
                var artist = jsonObject["response"]["artists"][index];
               
                relatedArtists.Add(ParseEchonestArtist(artist));
            }

            return relatedArtists;
        }


        public IList<string> SearchArtists(string term, int count = 15, int page = 1)
        {
            //TODO: Use a dedicated cache system for our artist page
            var cacheKey = string.Format("MOBSOCIAL_ARTIST_SEARCH_{0}_COUNT_{1}_PAGE_{2}", term, count, page);
            return _cacheProvider.Get(cacheKey, () => //cache for 24 hours
            {
                //http://developer.echonest.com/docs/v4/artist.html#search
                var apiUrl = GetApiUrl("artist/search", string.Format("name={0}&bucket=images&start={1}&results={2}", term, (count * (page - 1)), count));

                var responseBytes = HttpHelper.ExecuteGet(apiUrl);
                if (responseBytes == null || responseBytes.Length == 0)
                    return null;
                var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));
                var searchResults = new List<string>();

                for (int index = 0; index < jsonObject["response"]["artists"].Count(); index++)
                {
                    var artist = jsonObject["response"]["artists"][index];

                    searchResults.Add(ParseEchonestArtist(artist));
                }
                return searchResults;
            }, 24 * 60);           

        }

        public IList<string> GetArtistSongs(string artistName, int count = 15, int page = 1)
        {
            var songsResults = SearchSongs("", artistName, count, page);
            return songsResults;
        }
        public string GetRemoteSong(string remoteEntityId)
        {
            //http://developer.echonest.com/docs/v4/song.html#profile
            var apiUrl = GetApiUrl("song/profile", string.Format("track_id={0}&bucket=id:7digital-US&bucket=tracks", remoteEntityId));

            var responseBytes = HttpHelper.ExecuteGet(apiUrl);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));

            var song = jsonObject["response"]["songs"][0]["tracks"][0];
            return ParseEchonestTrack(song, jsonObject["response"]["songs"][0]);
        }

        public IList<string> SearchSongs(string term, string artist = "", int count = 15, int page = 1)
        {            
            //TODO: Use a dedicated cache system for our song page
            var cacheKey = string.Format("MOBSOCIAL_SONGS_SEARCH_{0}_{1}_COUNT_{2}_PAGE_{3}", term, artist, count, page);
            return _cacheProvider.Get(cacheKey, () => //cache for 24 hours
            {
                //generate parameters string
                List<string> parameters = new List<string>() {
                    "bucket=id:7digital-US",
                    "bucket=tracks",
                    string.Format("start={0}", (count * (page - 1))),
                    string.Format("results={0}", count)
                };

                if (!string.IsNullOrWhiteSpace(term))
                    parameters.Add(string.Format("title={0}", term));
                if (!string.IsNullOrWhiteSpace(artist))
                    parameters.Add(string.Format("artist={0}", artist));

                //http://developer.echonest.com/docs/v4/song.html#search
                var apiUrl = GetApiUrl("song/search", string.Join("&", parameters));

                var responseBytes = HttpHelper.ExecuteGet(apiUrl);
                if (responseBytes == null || responseBytes.Length == 0)
                    return null;
                var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));
                var songsResults = new List<string>();

                for (int index = 0; index < jsonObject["response"]["songs"].Count(); index++)
                {
                    var song = jsonObject["response"]["songs"][index];
                    if (song["tracks"] != null && song["tracks"].Count() > 0)
                    {
                        //only songs which have tracks should be taken
                        for (int subindex = 0; subindex < song["tracks"].Count(); subindex++)
                        {
                            var track = song["tracks"][subindex];
                            songsResults.Add(ParseEchonestTrack(track, song));
                        }

                    }

                }
                return songsResults;
            }, 24 * 60);   
        }

        public IList<string> GetSimilarSongs(string trackId, int count = 5)
        {
            //http://developer.echonest.com/docs/v4/basic.html#basic
            var apiUrl = GetApiUrl("playlist/basic", string.Format("track_id={0}&results={1}&bucket=id:7digital-US&bucket=tracks&type=song-radio", trackId, count));

            var responseBytes = HttpHelper.ExecuteGet(apiUrl);
            if (responseBytes == null || responseBytes.Length == 0)
                return null;
            var jsonObject = (JObject)JsonConvert.DeserializeObject(Encoding.ASCII.GetString(responseBytes));
            var songsResults = new List<string>();

            for (int index = 0; index < jsonObject["response"]["songs"].Count(); index++)
            {
                var song = jsonObject["response"]["songs"][index];
                if (song["tracks"] != null && song["tracks"].Count() > 0)
                {
                    //only songs which have tracks should be taken
                    for (int subindex = 0; subindex < song["tracks"].Count(); subindex++)
                    {
                        var track = song["tracks"][subindex];
                        songsResults.Add(ParseEchonestTrack(track, song));
                    }

                }

            }
            return songsResults;
        }


        #region Parsers
        /// <summary>
        /// Parses echonest json artist data to a generic artist json data. Can have different implementation for a different API
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        string ParseEchonestArtist(JToken artist)
        {
            string name = artist["name"].ToString();
            string imageUrl = "";

            if (artist["images"].Any())
            {

                imageUrl = artist["images"][0]["url"].ToString(); //first url in sequence
            }

            string yearsActive = "";
            if (artist["years_active"] != null && artist["years_active"].Count() > 0)
            {
                yearsActive = artist["years_active"][0]["start"].ToString(); //first year in sequence
            }

            string biography = "";
            if (artist["biographies"] != null && artist["biographies"].Count() > 0)
            {
                //find first data biography
                biography = artist["biographies"][0]["text"].ToString();
            }
            string location = "";
            if (artist["artist_location"] != null)
            {
                location = artist["artist_location"]["location"].ToString();
            }
            string remoteEntityId = artist["id"].ToString();
            //create a new json object which will be converted to string

            var responseObject = JObject.FromObject(new {
                Name = name,
                Description = biography,
                ImageUrl = imageUrl,
                HomeTown = location,
                ActiveSince = yearsActive,
                RemoteEntityId = remoteEntityId,
                RemoteSourceName = "EchoNest",
                Gender = "",
                ShortDescription = ""
            });
            return responseObject.ToString(Formatting.None);
        }

        /// <summary>
        /// Parses echonest json track data to a generic track json data. Can have different implementation for a different API
        /// </summary>        
        string ParseEchonestTrack(JToken track, JToken song)
        {
            string title = song["title"].ToString();
            string imageUrl = "";
            string previewUrl = "";
            string foreignId = "::";//safe initialization: TODO: put a check below for the same
            string foreignReleaseId = "::";

            string id = track["id"].ToString();
            string artistId = song["artist_id"].ToString();
            string artistName = "";

            if (song["artist_name"] != null)
            {
                artistName = song["artist_name"].ToString();
            }

            if (track["release_image"] != null)
            {
                imageUrl = track["release_image"].ToString();
            }


            if (track["preview_url"] != null)
            {
                previewUrl = track["preview_url"].ToString();
            }


            if (track["foreign_id"] != null)
            {
                foreignId = track["foreign_id"].ToString();
            }
            if (track["foreign_release_id"] != null)
            {
                foreignReleaseId = track["foreign_release_id"].ToString();
            }

            if (track["id"] != null)
            {
                id = track["id"].ToString();
            }

            var responseObject = JObject.FromObject(new {
                RemoteEntityId = id,
                RemoteSourceName = "Echonest",
                Name = title,
                ImageUrl = imageUrl,
                PreviewUrl = previewUrl,
                ForeignId = foreignId,
                TrackId = foreignId.Split(':')[2],  //7digital-US:track:3890387
                ReleaseId = foreignReleaseId.Split(':')[2],
                ArtistId = artistId,
                Description = "",
                ArtistName = artistName
            });

            return responseObject.ToString(Formatting.None);
        }


        #endregion

      
       


      
    }
}
