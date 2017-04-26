using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.EventPages;

namespace mobSocial.Services.EventPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IEventPageService : IBaseEntityService<EventPage>
    {
        List<EventPage> GetAllUpcomingEvents();
    }

}
