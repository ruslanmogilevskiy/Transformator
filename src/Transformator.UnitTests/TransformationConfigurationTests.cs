using System;
using NUnit.Framework;
using Rumo.Transformator.Configuration;
using Rumo.Transformator.UnitTests.TestHelpers;

namespace Rumo.Transformator.UnitTests
{
    [TestFixture]
    public class TransformationConfigurationTests : TestBase
    {
        [Test]
        public void Clone_ReturnClonedConfiguration()
        {
            var config = new TransformationConfiguration
            {
                InstanceFactory = _ => null,
                AutoCreateDestination = false
            };

            var result = config.Clone();

            Assert.IsNotNull(result);
            Assert.AreEqual(config.AutoCreateDestination, result.AutoCreateDestination);
            Assert.AreEqual(config.InstanceFactory, result.InstanceFactory);
            config.InstanceFactory = _ => new object();
            Assert.AreNotEqual(config.InstanceFactory, result.InstanceFactory);
        }

        [Test]
        public void Default_SetToNullValue_ThrowException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => TransformationConfiguration.Default = null);

            Assert.IsNotNull(exception);
            Assert.AreEqual("value".GetArgumentNullExceptionMessage(), exception.Message);
        }

        [Test]
        public void Default_SetToNotNullValue_UpdateDefaultConfiguration()
        {
            var config = new TransformationConfiguration();

            TransformationConfiguration.Default = config;

            Assert.AreEqual(config, TransformationConfiguration.Default);
        }
    }
}