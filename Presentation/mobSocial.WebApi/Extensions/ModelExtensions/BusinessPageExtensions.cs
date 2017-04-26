using mobSocial.Data.Entity.BusinessPages;
using mobSocial.WebApi.Models.BusinessPages;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class BusinessPageExtensions
    {
        public static BusinessPageModel ToModel(this BusinessPage businessPage)
        {
            var model = new BusinessPageModel()
            {
                Id = businessPage.Id,
                Name = businessPage.Name,
                Address1 = businessPage.Address1,
                Address2 = businessPage.Address2,
                City = businessPage.City,
                ZipPostalCode = businessPage.ZipPostalCode,
                CountryId = businessPage.CountryId,
                Phone = businessPage.Phone,
                Email = businessPage.Email,
                Website = businessPage.Website,
                StartDate = businessPage.StartDate,
                EndDate = businessPage.EndDate,
                Description = businessPage.Description,
                MetaKeywords = businessPage.MetaKeywords,
                MetaDescription = businessPage.MetaDescription,
            };
            return model;
        }
    }
}