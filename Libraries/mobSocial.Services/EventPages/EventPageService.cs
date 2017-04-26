using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.EventPages;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Settings;

namespace mobSocial.Services.EventPages
{
    public class EventPageService : BaseEntityService<EventPage>, IEventPageService
    {
       

        public List<EventPage> GetAllUpcomingEvents()
        {
            return base.Repository.Get(x => x.EndDate >= DateTime.Now || !x.EndDate.HasValue).ToList();
        }

        public EventPageService(IDataRepository<EventPage> dataRepository) : base(dataRepository) { }
    }

}
