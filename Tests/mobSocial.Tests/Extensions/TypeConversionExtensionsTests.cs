

using System;
using NUnit.Framework;
using mobSocial.Data.Extensions;

namespace mobSocial.Tests.Extensions
{
    [TestFixture]
    public class TypeConversionExtensionsTests
    {
        [Test]
        public void Can_Get_Boolean()
        {
            object obj = true;
            
            Assert.AreEqual(true, obj.GetBoolean());

            obj = 1;
            Assert.Throws<FormatException>(() => obj.GetBoolean());
            Assert.AreEqual(false, obj.GetBoolean(false));

            obj = -1;
            Assert.Throws<FormatException>(() => obj.GetBoolean());
            Assert.AreEqual(false, obj.GetBoolean(false));

            obj = "1";
            Assert.Throws<FormatException>(() => obj.GetBoolean());

            Assert.AreEqual(false, obj.GetBoolean(false));

            obj = "t";
            Assert.Throws<FormatException>(() => obj.GetBoolean());
            Assert.AreEqual(false, obj.GetBoolean(false));

            obj = "true";
            Assert.AreEqual(true, obj.GetBoolean());

            obj = "TRUE";
            Assert.AreEqual(true, obj.GetBoolean());

            obj = "FALSE";
            Assert.AreEqual(false, obj.GetBoolean());

            obj = "false";
            Assert.AreEqual(false, obj.GetBoolean());

            obj = 0;
            Assert.Throws<FormatException>(() => obj.GetBoolean());
            Assert.AreEqual(false, obj.GetBoolean(false));

            obj = new
            {
                test = 1
            };
            Assert.Throws<FormatException>(() => obj.GetBoolean());
            Assert.AreEqual(false, obj.GetBoolean(false));
        }

        [Test]
        public void Can_Get_Integer()
        {
            int x = 5;
            Assert.AreEqual(5, x.GetInteger());

            decimal d = 5.5m;
            Assert.Throws<FormatException>(() => d.GetInteger());
            Assert.AreEqual(0, d.GetInteger(false));

            string s = "test";
            Assert.Throws<FormatException>(() => s.GetInteger());
            Assert.AreEqual(0, s.GetInteger(false));

            bool v = true;
            Assert.Throws<FormatException>(() => v.GetInteger());
            Assert.AreEqual(0, v.GetInteger(false));
        }

        [Test]
        public void Can_Get_Decimal()
        {
            int x = 5;
            Assert.AreEqual(5, x.GetDecimal());

            decimal d = 5.5m;
            Assert.AreEqual(5.5m, d.GetDecimal());

            string s = "test";
            Assert.Throws<FormatException>(() => s.GetDecimal());
            Assert.AreEqual(0, s.GetDecimal(false));

            bool v = true;
            Assert.Throws<FormatException>(() => v.GetDecimal());
            Assert.AreEqual(0, v.GetDecimal(false));
        }

    }
}