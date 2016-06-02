using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Social;
using mobSocial.Services.Settings;
using Mob.Core;
using Mob.Core.Data;
using Mob.Core.Services;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Plugin.WebApi.MobSocial.Domain;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace mobSocial.Services.Battles
{
    public class CustomerFavoriteSongService : BaseEntityService<CustomerFavoriteSong>, ICustomerFavoriteSongService
    {
        private MediaSettings _nopMediaSettings;
        private IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private IOAuthService _oAuthService;
        private IWorkContext _workContext;

        public CustomerFavoriteSongService(ISettingService settingService, IWebHelper webHelper,
            ILogger logger, IEventPublisher eventPublisher,
            IDataRepository<CustomerFavoriteSong> entityRepository,
            MediaSettings mediaSettings,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            IOAuthService oAuthService,
            IWorkContext workContext) : base(entityRepository, workContext, urlRecordService)
        {
            _nopMediaSettings = mediaSettings;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _workContext = workContext;
            _oAuthService = oAuthService;
        }


        public List<CustomerFavoriteSong> GetTop10(int customerId)
        {
            var top10FavoriteSongs = Repository.Table
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.DisplayOrder)
                .Take(10)
                .ToList();

            var oAuthBase = new OAuthBase();
            
            top10FavoriteSongs.ForEach(
                    x => oAuthBase.GetSignedUrl(x.PreviewUrl, 
                        _oAuthService.ConsumerKey, 
                        _oAuthService.ConsumerSecret));
            

            return top10FavoriteSongs;

        }

        public void UpdateFavoriteSongOrder(int favoriteSongId, int displayOrder)
        {
            throw new NotImplementedException();
        }

        public override List<CustomerFavoriteSong> GetAll(string term, int count = 15, int page = 1)
        {
            throw new NotImplementedException();
        }
    }

}
