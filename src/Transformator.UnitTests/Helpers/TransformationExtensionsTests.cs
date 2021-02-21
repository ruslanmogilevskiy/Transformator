using NUnit.Framework;
using Transformator.Configuration;
using Transformator.Helpers;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests.Helpers
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