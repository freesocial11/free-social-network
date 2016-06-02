using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Songs;

namespace mobSocial.Services.Songs
{
    public class SharedSongService: MobSocialEntityService<SharedSong>, ISharedSongService
    {
        private IDataRepository<Song> _songsRepository;
        public SharedSongService(IDataRepository<Song> songsRepository, IDataRepository<SharedSong> sharedSongsRepository)
            : base(sharedSongsRepository)
        {
            _songsRepository = songsRepository;
        }

        public IList<SharedSong> GetSharedSongs(int customerId,int count = 15, int page = 1)
        {

            int totalPages;
            return GetSharedSongs(customerId, out totalPages, count, page);
        }

        public IList<SharedSong> GetSharedSongs(int customerId, out int totalPages, int count = 15, int page = 1)
        {
            var songRows = base.Repository.Get(x => true).OrderByDescending(x => x.Id).AsQueryable();
            var listSongShared = songRows.Where(x => x.SenderId == customerId);
            totalPages = int.Parse(Math.Ceiling((decimal)songRows.Count() / count).ToString());
            return listSongShared.Skip((page - 1) * count).Take(count).ToList();
        }

     

        public IList<SharedSong> GetReceivedSongs(int customerId, int count = 15, int page = 1)
        {
            int totalPages;
            return GetReceivedSongs(customerId, out totalPages, count, page);
        }

        public IList<SharedSong> GetReceivedSongs(int customerId, out int totalPages, int count = 15, int page = 1)
        {
            var songRows = base.Repository.Get(x => true).OrderByDescending(x => x.Id).AsQueryable();
            var listSongShared = songRows.Where(x => x.CustomerId == customerId);
            totalPages = int.Parse(Math.Ceiling((decimal)songRows.Count() / count).ToString());
            return listSongShared.Skip((page - 1) * count).Take(count).ToList();
        }
    }
}
