using System.Collections.Generic;
using Mob.Core.Services;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace mobSocial.Services.Battles
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IEventPageService : IBaseEntityService<EventPage, EventPagePicture>
    {
        List<EventPage> GetAllUpcomingEvents();
    }

}
