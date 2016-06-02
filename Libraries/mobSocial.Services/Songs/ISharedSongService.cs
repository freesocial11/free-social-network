using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Songs;

namespace mobSocial.Services.Songs
{
    public interface ISharedSongService: IBaseEntityService<SharedSong>
    {
        
        IList<SharedSong> GetSharedSongs(int customerId, int count = 15, int page = 1);

        IList<SharedSong> GetSharedSongs(int customerId, out int totalPages, int count = 15, int page = 1);

        IList<SharedSong> GetReceivedSongs(int customerId, int count = 15, int page = 1);

        IList<SharedSong> GetReceivedSongs(int customerId, out int totalPages , int count = 15, int page = 1);
    }
}
