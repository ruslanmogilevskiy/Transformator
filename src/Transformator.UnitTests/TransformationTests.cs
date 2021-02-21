using NUnit.Framework;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests
{
    [TestFixture]
    public class TransformationTests : TestBase
    {
        [Test]
        public void For_ReturnTransformationBuilderWithClonedDefaultConfiguration()
        {
            TransformationConfiguration.Default.InstanceFactory = _ => null;
            TransformationConfiguration.Default.AutoCreateDestination = true;
            TransformationConfiguration.Default.IsolateInitialDestination = true;

            var result = Transformation.For<Foo, Bar>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Configuration);
            Assert.AreEqual(TransformationConfiguration.Default.InstanceFactory, result.Configuration.InstanceFactory);
            Assert.AreEqual(TransformationConfiguration.Default.AutoCreateDestination, result.Configuration.AutoCreateDestination);
            Assert.AreEqual(TransformationConfiguration.Default.IsolateInitialDestination, result.Configuration.IsolateInitialDestination);
        }
    }
}