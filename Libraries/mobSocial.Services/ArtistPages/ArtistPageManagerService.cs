using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.ArtistPages;

namespace mobSocial.Services.ArtistPages
{
    public class ArtistPageManagerService: MobSocialEntityService<ArtistPageManager>, IArtistPageManagerService
    {
        private readonly IDataRepository<ArtistPageManager> _managerRepository;
        private readonly IDataRepository<ArtistPage> _pageRepository;

        public ArtistPageManagerService(IDataRepository<ArtistPageManager> managerRepository,
            IDataRepository<ArtistPage> pageRepository)
            : base(managerRepository)
        {
            _managerRepository = managerRepository;
            _pageRepository = pageRepository;
        }

        public void AddPageManager(ArtistPageManager manager)
        {
            _managerRepository.Insert(manager);
        }

        public void DeletePageManager(ArtistPageManager manager)
        {
            _managerRepository.Delete(manager);
        }

        public void DeletePageManager(int artistPageId, int customerId)
        {
            _managerRepository.Delete(x => x.ArtistPageId == artistPageId && x.CustomerId == customerId);
        }

        public bool IsPageManager(int artistPageId, int customerId)
        {
            return _managerRepository.Get(x => x.ArtistPageId == artistPageId && x.CustomerId == customerId).Any();
        }

        public IList<ArtistPageManager> GetPageManagers(int artistPageId)
        {
            return _managerRepository.Get(x => x.ArtistPageId == artistPageId).ToList();
        }

        public IList<ArtistPage> GetPagesAsManager(int customerId)
        {
            return _managerRepository.Get(x => x.CustomerId == customerId).Select( x=> x.ArtistPage).ToList();
        }


      
    }
}
