using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Enum;
using mobSocial.Data.Interfaces;
using mobSocial.Services.MediaServices;

namespace mobSocial.Services.Extensions
{
    public static class PictureExtensions
    {
        public static IList<Media> GetPictures<T>(this IPicturesSupported<T> pictureSupportedInstance) where T: BaseEntity
        {
            //resolve picture service to retrieve pictures
            var pictureService = mobSocialEngine.ActiveEngine.Resolve<IMediaService>();
            return pictureService.GetEntityMedia<T>(pictureSupportedInstance.Id, MediaType.Image);
        }
    }
}