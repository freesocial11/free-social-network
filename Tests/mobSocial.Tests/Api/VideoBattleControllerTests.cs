using System;
using mobSocial.Data.Enum;
using mobSocial.Tests.SampleData;
using mobSocial.Tests.Setup;
using mobSocial.Tests.Setup.Extensions;
using mobSocial.WebApi.Controllers;
using mobSocial.WebApi.Models.Battles;
using NUnit.Framework;


namespace mobSocial.Tests.Api
{
    [TestFixture]
    public class VideoBattleControllerTests : MobSocialTestCase
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            RegisterController<VideoBattleController>();

            //login a test user
            Login();

        }
        [Test]
        [Order(0)]
        public void SaveVideoBattle_fails_for_invalid_model()
        {
            using (var battleController = ResolveController<VideoBattleController>())
            {
                var videoBattleModel = new VideoBattleModel();
                battleController.Validate(videoBattleModel);

                var response = battleController.SaveVideoBattle(videoBattleModel);
                Assert.IsFalse(response.GetValue<bool>("Success"));
            }
        }

        [Test]
        [Order(1)]
        public void SaveVideoBattle_succeeds_for_valid_model()
        {
            using (var battleController = ResolveController<VideoBattleController>())
            {
                var videoBattleModel = SampleVideoBattles.GetSampleVideoBattleModel(0);
                var response = battleController.SaveVideoBattle(videoBattleModel);
                Assert.IsTrue(response.GetValue<bool>("Success"));
            }
        }

        [Test]
        [Order(2)]
        public void Get_succeeds()
        {
            using (var battleController = ResolveController<VideoBattleController>())
            {
                const int id = 1;
                var response = battleController.Get(id);
                Assert.IsTrue(response.GetValue<bool>("Success"));

                var retrievedId = response.GetValue<dynamic>("ResponseData").Id;
                //retrieve the data
                Assert.AreEqual(id, retrievedId);
            }
        }

        [Test]
        [Order(3)]
        public void GetBattles_with_empty_model_succeeds()
        {
            using (var battleController = ResolveController<VideoBattleController>())
            {
                //save some battles first
                foreach(var battle in SampleVideoBattles.GetSampleVideoBattles())
                    SaveEntity(battle);

                var queryModel = new VideoBattleQueryModel();

                var response = battleController.GetBattles(queryModel);
                Assert.IsTrue(response.GetValue<bool>("Success"));

                var count = response.GetValue<dynamic>("ResponseData").VideoBattles.Count;
                Assert.AreEqual(SampleVideoBattles.GetSampleVideoBattles().Count, count);
            }
        }

        [Test]
        public void GetBattles_with_pagination_model_succeeds()
        {
            using (var battleController = ResolveController<VideoBattleController>())
            {
                //save some battles first
                foreach (var battle in SampleVideoBattles.GetSampleVideoBattles())
                    SaveEntity(battle);

                var queryModel = new VideoBattleQueryModel
                {
                  // Count = 1
                };

                var response = battleController.GetBattles(queryModel);
                Assert.IsTrue(response.GetValue<bool>("Success"));

                var count = response.GetValue<dynamic>("ResponseData").VideoBattles.Count;
                Assert.AreEqual(1, count);
            }
        }

    }
}