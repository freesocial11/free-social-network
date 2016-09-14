using System.Linq;
using mobSocial.Data.Attributes;
using mobSocial.Data.Entity.Tokens;
using mobSocial.Services.Tokens;
using mobSocial.Tests.Setup;
using Newtonsoft.Json;
using NUnit.Framework;

namespace mobSocial.Tests.Services
{
    [TestFixture]
    public class TokenProcessorTests : MobSocialTestCase
    {
        private ITokenProcessor _tokenProcessor;
        [OneTimeSetUp]
        public void Initialize()
        {
            _tokenProcessor = Resolve<ITokenProcessor>();
        }

        [Test]
        public void GetAvailableTokens_ForAvailable_Succeeds()
        {
            var availableTokens = _tokenProcessor.GetAvailableTokens<TestField1>(null);
            Assert.AreEqual(1, availableTokens.Count);
        }

        [Test]
        public void GetAvailableTokens_ForUnavailable_Succeeds()
        {
            var availableTokens = _tokenProcessor.GetAvailableTokens<TestField2>(null);
            Assert.AreEqual(null, availableTokens);
        }

        [Test]
        public void GetAvailableTokens_ForAvailable_WithInstance_Succeeds()
        {
            var availableTokens = _tokenProcessor.GetAvailableTokens(new TestField1()
            {
                IntField = 1,
                StringField = "some value"
            });

            var expectedToken = JsonConvert.SerializeObject(new Token("{{TestField1.StringField}}", "some value"));
            var actualToken = JsonConvert.SerializeObject(availableTokens.First());
            Assert.AreEqual(expectedToken, actualToken);
        }

        [Test]
        public void ProcessTokens_Succeeds()
        {
            var content = "Some content with {{TestField1.StringField}}";

            var processed = _tokenProcessor.ProcessTokens(content, new TestField1()
            {
                IntField = 1,
                StringField = "some value"
            });

            Assert.AreEqual("Some content with some value", processed);
        }

        [Test]
        public void ProcessAllTokens_Succeeds()
        {
            var content = "Some content with {{TestField1.StringField}} and {{TestField3.IntField}}";

            var tf1 = new TestField1()
            {
                IntField = 1,
                StringField = "some value"
            };

            var tf3 = new TestField3() {
                IntField = 5,
                StringField = "some other value"
            };
            var processed = _tokenProcessor.ProcessAllTokens(content, tf1, tf3);

            Assert.AreEqual("Some content with some value and 5", processed);
        }

        public class TestField1
        {
            [TokenField]
            public string StringField { get; set; }

            public int IntField { get; set; }
        }

        public class TestField2
        {
            public string StringField { get; set; }

            public int IntField { get; set; }
        }

        public class TestField3
        {
            public string StringField { get; set; }
            [TokenField]
            public int IntField { get; set; }
        }
    }
}
