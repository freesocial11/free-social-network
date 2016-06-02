using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.ArtistPages;
using mobSocial.Data.Entity.Songs;

namespace mobSocial.Services.Songs
{
    public class SongService : MobSocialEntityService<Song>, ISongService
    {
        #region Fields
        private readonly IDataRepository<ArtistPage> _artistPageRepository;
        #endregion

        #region Constructor
        public SongService(IDataRepository<Song> dataRepository, IDataRepository<ArtistPage> artistPageRepository) : base(dataRepository)
        {
            _artistPageRepository = artistPageRepository;
        }

        #endregion


        public IList<Song> SearchSongs(string term, int count = 15, int page = 1, bool searchDescriptions = false, bool searchArtists = false, string artistName = "", bool publishedOnly = true)
        {
            int totalCount;
            return SearchSongs(term, out totalCount, count, page, searchDescriptions, searchArtists, artistName, publishedOnly);
        }

        public IList<Song> SearchSongs(string term, out int totalPages, int count = 15, int page = 1, bool searchDescriptions = false, bool searchArtists = false, string artistName = "", bool publishedOnly = true)
        {
            var songRows = base.Repository.Get(x => true).OrderBy(x => x.Id).AsQueryable();
            var listSongs = new List<Song>();

            listSongs = listSongs.Union(songRows.Where(x => x.Name.Contains(term)).ToList()).ToList();

            if (searchDescriptions)
            {
                
                listSongs = listSongs.Union(songRows.Where(x=>x.Description.Contains(term))).ToList();
            }
            if (searchArtists)
            {

               //we first get all the remote artist ids which contain our artist name in their artist name column
               var remoteArtistIds = _artistPageRepository.Get(x => x.Name.Contains(artistName)).Select(x => x.RemoteEntityId);

              //now filter those rows which belong to these artists only
               listSongs = listSongs.Where(x => remoteArtistIds.Contains(x.RemoteArtistId)).ToList();
            }          

            if (publishedOnly)
                listSongs = listSongs.Where(x => x.Published == true).ToList();

            totalPages = int.Parse(Math.Ceiling((decimal)listSongs.Count() / count).ToString());

            return listSongs.Skip((page - 1) * count).Take(count).ToList();
        }

        public Song GetSongByRemoteEntityId(string remoteEntityId)
        {
            return base.Repository.Get(x => remoteEntityId.Contains(x.RemoteEntityId)).FirstOrDefault();
        }




        public Song GetSongByProductId(int productId)
        {
            return base.Repository.Get(x => x.AssociatedProductId == productId).FirstOrDefault();
        }


        
    }
}
