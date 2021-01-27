using NUnit.Framework;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformatorBuilderTests
    {
        [Test]
        public void For_ReturnTransformationBuilderWithClonedDefaultConfiguration()
        {
            TransformationConfiguration.Default.InstanceFactory = _ => null;

            var result = TransformatorBuilder.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Configuration);
            Assert.AreEqual(TransformationConfiguration.Default.InstanceFactory, result.Configuration.InstanceFactory);
        }
    }
}