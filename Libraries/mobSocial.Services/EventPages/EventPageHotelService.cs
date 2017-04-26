using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.EventPages;

namespace mobSocial.Services.EventPages
{
    public class EventPageHotelService : BaseEntityService<EventPageHotel>,
        IEventPageHotelService
    {

        public EventPageHotelService(IDataRepository<EventPageHotel> repository) : base(repository)
        {
        }

        public List<EventPageHotel> GetAll(int eventPageId)
        {
            return Repository.Get(x => x.EventPageId == eventPageId).ToList();
        }
    }

}
