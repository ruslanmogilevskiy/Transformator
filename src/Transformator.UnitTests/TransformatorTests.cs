using NUnit.Framework;
using Transformator.Models;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformatorTests
    {
        [TearDown]
        public void TearDown()
        {
            Transformator.DefaultSettings = null;
        }

        [Test]
        public void For_DefaultSettingsAreNull_ReturnTransformationBuilderWithNullSettings()
        {
            Transformator.DefaultSettings = null;

            var result = Transformator.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNull(result.Settings);
        }

        [Test]
        public void For_DefaultSettingsAreNotNull_ReturnTransformationBuilderWithDefaultSettings()
        {
            Transformator.DefaultSettings = new TransformationSettings();

            var result = Transformator.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.AreEqual(Transformator.DefaultSettings, result.Settings);
        }
    }
}