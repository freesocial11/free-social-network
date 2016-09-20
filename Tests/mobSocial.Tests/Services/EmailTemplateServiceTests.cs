using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Emails;
using mobSocial.Services.Emails;
using mobSocial.Tests.Setup;
using NUnit.Framework;

namespace mobSocial.Tests.Services
{
    [TestFixture]
    public class EmailTemplateServiceTests : MobSocialTestCase
    {
        private IEmailTemplateService _emailTemplateService;
        [OneTimeSetUp]
        public void Initialize()
        {
            _emailTemplateService = Resolve<IEmailTemplateService>();
        }

        [Test]
        public void GetProcessedContentTemplate_Succeeds()
        {
            var e1 = new EmailTemplate()
            {
                IsMaster = true,
                Template = "<i>" + EmailTokenNames.MessageContent + "</i>"
            };
            SaveEntity(e1);

            var e2 = new EmailTemplate() {
                IsMaster = true,
                Template = "<span>" + EmailTokenNames.MessageContent + "</span>",
                ParentEmailTemplateId = e1.Id
            };
            SaveEntity(e2);

            var e3 = new EmailTemplate() {
                IsMaster = false,
                Template = "<b>Some Content</b>",
                ParentEmailTemplateId = e2.Id
            };
            SaveEntity(e3);

            var content = _emailTemplateService.GetProcessedContentTemplate(e3);
            var expected = "<i><span><b>Some Content</b></span></i>";
            Assert.AreEqual(expected, content);

        }
    }
}
