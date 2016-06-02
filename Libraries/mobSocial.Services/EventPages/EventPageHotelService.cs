using System;
using System.Collections.Generic;
using System.Linq;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace mobSocial.Services.Battles
{
    public class EventPageHotelService : BaseEntityService<EventPageHotel>,
        IEventPageHotelService
    {

        public EventPageHotelService(IDataRepository<EventPageHotel> repository,
            IWorkContext workContext) : base(repository)
        {
        }

        public List<EventPageHotel> GetAll(int eventPageId)
        {
            return base.Repository.Table
                .Where(x => x.EventPageId == eventPageId)
                .ToList();
        }

        public override List<EventPageHotel> GetAll(string term, int count = 15, int page = 1)
        {
            // TODO: Later make a stored procedure.
            return base.Repository.Table
                .Where(x => x.Name.ToLower().Contains(term.ToLower()))
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();
        }
    }

}
