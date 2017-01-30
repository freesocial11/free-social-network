using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services.Events;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Users;
using mobSocial.Tests.Setup;
using NUnit.Framework;

namespace mobSocial.Tests.Services
{
    [TestFixture]
    public class EventPublisherServiceTests : MobSocialTestCase
    {
        private IUserService _userService;
        [SetUp]
        public void Initialize()
        {
            mobSocialEngine.ActiveEngine.IocContainer.Register<IEventConsumer<User>, UserEventTestConsumer>();
            
            _userService = Resolve<IUserService>();
        }

        [Test]
        public void Does_event_publisher_for_insert_works()
        {
            var user = GetTestUser();
            _userService.Insert(user);
        }

        [Test]
        public void Does_event_publisher_for_delete_works()
        {
            _userService.Delete(x => x.Id == 2);
        }
    }

    public class UserEventTestConsumer : IEventConsumer<User>
    {
        public void EntityInserted(User entity)
        {
            Assert.NotNull(entity, "Event insert executed");
        }

        public void EntityUpdated(User entity)
        {
            
        }

        public void EntityDeleted(User entity)
        {
            Assert.AreEqual(2, entity.Id, "Entity failed to delete");
        }
    }
}
