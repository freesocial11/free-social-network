using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Education;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Models.Education;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class SchoolExtensions
    {
        public static SchoolResponseModel ToModel(this School school, IMediaService _mediaService)
        {
            var model = new SchoolResponseModel()
            {
                Id = school.Id,
                Name = school.Name,
                City = school.City,
                LogoUrl = _mediaService.GetPictureUrl(school.LogoId, PictureSizeNames.ThumbnailImage, true)
            };
            return model;
        }

        public static SchoolResponseModel ToEntityModel(this School school, IMediaService _mediaService)
        {
            var model = new SchoolResponseModel() {
                Id = school.Id,
                Name = school.Name,
                City = school.City,
                LogoUrl = _mediaService.GetPictureUrl(school.LogoId, PictureSizeNames.ThumbnailImage, true)
            };
            return model;
        }
    }
}