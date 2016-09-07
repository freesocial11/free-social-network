using System;
using System.Web.Http.Results;
using mobSocial.Data.Entity.Education;
using mobSocial.Data.Enum;
using mobSocial.Tests.Setup;
using mobSocial.Tests.Setup.Extensions;
using mobSocial.WebApi.Controllers;
using mobSocial.WebApi.Models.Education;
using NUnit.Framework;

namespace mobSocial.Tests.Api
{
    [TestFixture]
    public class EducationControllerTests : MobSocialTestCase
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            RegisterController<EducationController>();

            //login a test user
            Login();
        }

        [Test]
        public void Post_invalid_model_fails()
        {
            using (var controller = ResolveController<EducationController>())
            {
                var model = new EducationEntityModel();
                controller.Validate(model);
                var response = controller.Post(model);
                Assert.IsInstanceOf<BadRequestResult>(response);
            }
        }

        [Test]
        public void Post_valid_model_succeeds()
        {
            using (var controller = ResolveController<EducationController>())
            {
                var school = new School()
                {
                    Name = "Test School",
                };
                SaveEntity(school);
                var model = new EducationEntityModel()
                {
                    Description = "Test description",
                    Name = "Test Education",
                    FromDate = DateTime.UtcNow,
                    EducationType = EducationType.College,
                    ToDate = null,
                    SchoolId = school.Id
                };
                controller.Validate(model);
                var response = controller.Post(model);
                Assert.IsTrue(response.GetValue<bool>("Success"));
            }
        }
        
    }
}
