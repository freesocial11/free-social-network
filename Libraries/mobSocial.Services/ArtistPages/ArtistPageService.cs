using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.ArtistPages;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Permalinks;

namespace mobSocial.Services.ArtistPages
{
    public class ArtistPageService: MobSocialEntityService<ArtistPage>, IArtistPageService
    {
        private IPermalinkService _permalinkervice;
        private IMediaService _pictureService;

        public ArtistPageService(IDataRepository<ArtistPage> dataRepository, 
            IPermalinkService permalinkervice, 
            IMediaService pictureService) : base(dataRepository)
        {
            _permalinkervice = permalinkervice;
            _pictureService = pictureService;
        }

        public ArtistPage GetArtistPageByName(string name)
        {
            return Repository.Get(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }

        public IList<ArtistPage> GetArtistPagesByPageOwner(int pageOwnerId, string searchTerm = "", int count = 15, int page = 1, bool includeOrphan = false)
        {
            int totalCount;
            return GetArtistPagesByPageOwner(pageOwnerId, out totalCount, searchTerm, count, page, includeOrphan);
        }

       
        public IList<ArtistPage> SearchArtists(string term, int count = 15, int page = 1, bool searchDescriptions = false)
        {
            int totalCount;
            return SearchArtists(term, out totalCount, count, page, searchDescriptions);
        }


        public IList<ArtistPage> GetArtistPagesByPageOwner(int pageOwnerId, out int totalPages, string searchTerm = "", int count = 15, int page = 1, bool includeOrphan = false)
        {
            var artistPageRows = Repository.Get(x => true);
            if (includeOrphan)
            {
                artistPageRows = artistPageRows.Where(x => x.PageOwnerId == pageOwnerId || x.PageOwnerId == 0);
            }
            else
            {
                artistPageRows = artistPageRows.Where(x => x.PageOwnerId == pageOwnerId);

            }
            if (searchTerm != "")
            {
                artistPageRows = artistPageRows.Where(x=> x.Name.Contains(searchTerm));
            }
          
            totalPages = int.Parse(Math.Ceiling((decimal)artistPageRows.Count() / count).ToString());
            return artistPageRows.OrderByDescending(x => x.Id).Skip(count * (page - 1)).Take(count).ToList();
        }

        public IList<ArtistPage> SearchArtists(string term, out int totalPages, int count = 15, int page = 1, bool searchDescriptions = false)
        {
            var artistRows = Repository.Get(x => true).OrderBy(x => x.Id).AsQueryable();


            if (searchDescriptions)
            {
                artistRows = artistRows.Where(x => x.Name.Contains(term) || x.Biography.Contains(term));
            }
            else
            {
                artistRows = artistRows.Where(x => x.Name.Contains(term));
            }

            totalPages = int.Parse(Math.Ceiling((decimal)artistRows.Count() / count).ToString());

            return artistRows.Skip((page - 1) * count).Take(count).ToList();
        }

        public IList<ArtistPage> GetArtistPagesByRemoteEntityId(string[] remoteEntityId)
        {
            return base.Repository.Get(x => true).Where(x => remoteEntityId.Contains(x.RemoteEntityId)).ToList();
        }
    }
}
