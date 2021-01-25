using NUnit.Framework;
using Transformator.Models;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformatorBuilderTests
    {
        [TearDown]
        public void TearDown()
        {
            TransformatorBuilder.DefaultSettings = null;
        }

        [Test]
        public void DefaultSettings_SetNewInstance_SameInstanceReturnedFromGetter()
        {
            var settings = new TransformationSettings();

            TransformatorBuilder.DefaultSettings = settings;

            Assert.AreEqual(settings, TransformatorBuilder.DefaultSettings);
        }

        [Test]
        public void For_DefaultSettingsAreNull_ReturnTransformationBuilderWithNullSettings()
        {
            TransformatorBuilder.DefaultSettings = null;

            var result = TransformatorBuilder.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNull(result.Settings);
        }

        [Test]
        public void For_DefaultSettingsAreNotNull_ReturnTransformationBuilderWithDefaultSettings()
        {
            TransformatorBuilder.DefaultSettings = new TransformationSettings();

            var result = TransformatorBuilder.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.AreEqual(TransformatorBuilder.DefaultSettings, result.Settings);
        }
    }
}