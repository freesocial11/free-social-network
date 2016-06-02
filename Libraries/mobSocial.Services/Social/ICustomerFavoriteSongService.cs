using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface ICustomerFavoriteSongService : IBaseEntityService<UserFavoriteSong>
    {
        List<UserFavoriteSong> GetTop10(int customerId);

        void UpdateFavoriteSongOrder(int favoriteSongId, int displayOrder);

    }
}
