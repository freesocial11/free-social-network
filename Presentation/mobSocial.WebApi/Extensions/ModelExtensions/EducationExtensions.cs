using mobSocial.Data.Entity.Education;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Models.Education;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class EducationExtensions
    {
        public static EducationResponseModel ToModel(this Education education, IMediaService mediaService)
        {
            var model = new EducationResponseModel()
            {
                Name = education.Name,
                Description = education.Description,
                FromDate = education.FromDate,
                ToDate = education.ToDate,
                EducationType = education.EducationType,
                School = education.School?.ToModel(mediaService)
            };
            return model;
        }

        public static EducationEntityModel ToEntityModel(this Education education)
        {
            var model = new EducationEntityModel()
            {
                Name = education.Name,
                Description = education.Description,
                FromDate = education.FromDate,
                ToDate = education.ToDate,
                EducationType = education.EducationType,
                SchoolId = education.SchoolId,
                Id = education.Id,
                School = education.School?.ToModel(null)
            };
           
            return model;
        }
    }
}