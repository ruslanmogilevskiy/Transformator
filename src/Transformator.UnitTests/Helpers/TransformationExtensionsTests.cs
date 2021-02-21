using NUnit.Framework;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.Helpers;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests.Helpers
{
    [TestFixture]
    public class TransformationExtensionsTests
    {
        [Test]
        public void CreateInstanceSafe_ConfigurationAndInstanceFactoryNotNull_CallInstanceFactory_AndReturnResult()
        {
            var instance = new Foo();
            var configuration = new TransformationConfiguration { InstanceFactory = t => t == typeof(Foo) ? instance : null };

            var result = configuration.CreateInstanceSafe<Foo>();

            Assert.AreEqual(instance, result);
        }

        [Test]
        public void CreateInstanceSafe_ConfigurationNotNull_ButInstanceFactoryNull_CallActivator_AndReturnResult()
        {
            var configuration = new TransformationConfiguration { InstanceFactory = null };

            var result = configuration.CreateInstanceSafe<Foo>();

            Assert.IsNotNull(result);
            Assert.IsNull(result.SourceProperty);
        }
    }
}