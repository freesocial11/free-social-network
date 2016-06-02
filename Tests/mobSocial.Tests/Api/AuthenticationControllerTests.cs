using mobSocial.Tests.Setup;
using mobSocial.WebApi.Controllers;
using mobSocial.WebApi.Models.Authentication;
using NUnit.Framework;

namespace mobSocial.Tests.Api
{
    [TestFixture]
    public class AuthenticationControllerTests : MobSocialTestCase
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            RegisterController<AuthenticationController>();
        }

        [Test]
        public void Login_InvalidInfo_Fails()
        {
            using (var controller = Resolve<AuthenticationController>())
            {
                var response = controller.Login(new LoginModel() {
                    Email = "test@example.com",
                    Password = "askforitwrong"
                });
                var success = GetValueFromJsonResult<bool>(response, "Success");
                Assert.IsFalse(success);
            }

        }

        [Test]
        public void Login_ValidInfo_Succeeds()
        {
            //let's save the info first
            var user = GetTestUser();
            SaveAndLoadEntity(user);

            using (var controller = Resolve<AuthenticationController>())
            {
                var response = controller.Login(new LoginModel() {
                    Email = "test@example.com",
                    Password = "askforit"
                });

                var success = GetValueFromJsonResult<bool>(response, "Success");
                Assert.IsTrue(success);
            }
        }
    }
}