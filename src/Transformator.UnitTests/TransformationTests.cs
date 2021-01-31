using NUnit.Framework;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformationTests
    {
        [Test]
        public void For_DefaultConfigurationIsNotNull_ReturnTransformationBuilderWithClonedDefaultConfiguration()
        {
            TransformationConfiguration.Default = new TransformationConfiguration
            {
                InstanceFactory = _ => null,
                AutoCreateDestination = true
            };

            var result = Transformation.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Configuration);
            Assert.AreEqual(TransformationConfiguration.Default.InstanceFactory, result.Configuration.InstanceFactory);
            Assert.AreEqual(TransformationConfiguration.Default.AutoCreateDestination, result.Configuration.AutoCreateDestination);
        }

        [Test]
        public void For_DefaultConfigurationIsNull_ReturnTransformationBuilderWithNullConfiguration()
        {
            TransformationConfiguration.Default = null;

            var result = Transformation.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNull(result.Configuration);
        }
    }
}