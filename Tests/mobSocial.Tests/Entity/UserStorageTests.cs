using System.Data.Entity.Validation;
using NUnit.Framework;
using mobSocial.Tests.Database;

namespace mobSocial.Tests.Entity
{
    [TestFixture]
    public class UserStorageTests : DatabaseStorageTests
    {
        [Test]
        public void Does_Invalid_User_Persist_Fails()
        {

            var user = GetTestUser(true);
            Assert.Throws<DbEntityValidationException>(() => SaveAndLoadEntity(user));
        }

        [Test]
        public void Does_Valid_User_Persists()
        {

            var user = GetTestUser();
            var retrieved = SaveAndLoadEntity(user);
            Assert.AreEqual(user.Id, retrieved.Id);
        }
    }
}