using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace mobSocial.Services.Battles
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IEventPageHotelService : IBaseEntityService<EventPageHotel>
    {
        List<EventPageHotel> GetAll(int eventPageId);
    }

}
