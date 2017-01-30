using mobSocial.Data.Entity.Skills;
using mobSocial.Tests.SampleData;
using mobSocial.Tests.Setup;
using mobSocial.Tests.Setup.Extensions;
using mobSocial.WebApi.Controllers;
using mobSocial.WebApi.Models.Skills;
using NUnit.Framework;

namespace mobSocial.Tests.Api
{
    [TestFixture]
    public class SkillControllerTests : MobSocialTestCase
    {
        private int _skillsInserted = 0;

        [OneTimeSetUp]
        public void Initialize()
        {
            RegisterController<SkillController>();

            //login a test user
            Login();
        }

        [Test]
        public void Post_WithValidData_Succeeds()
        {
            using (var controller = ResolveController<SkillController>())
            {
                var skillEntityModel = new UserSkillEntityModel()
                {
                    SkillName = "New Skill",
                    UserId = 1,
                    Description = "some random description",
                    DisplayOrder = 1,
                    ExternalUrl = ""
                };
                var response = controller.Post(skillEntityModel);
                Assert.IsTrue(response.GetValue<bool>("Success"));
            }
        }

        [Test]
        public void Get_SystemSkills_Succeeds()
        {
            using (var controller = ResolveController<SkillController>())
            {
                SaveEntity(SampleSkills.GetSkill(0));
                var response = controller.GetSystemSkills();
                var skillsCount = response.GetValue<dynamic>("ResponseData").Skills.Count;
                Assert.AreEqual(1, skillsCount);
            }
        }
    }
}