using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GeoServices;

namespace mobSocial.Services.GeoServices
{
    public class StateService : BaseEntityService<State>, IStateService
    {
        public StateService(IDataRepository<State> dataRepository) : base(dataRepository) { }
    }
}