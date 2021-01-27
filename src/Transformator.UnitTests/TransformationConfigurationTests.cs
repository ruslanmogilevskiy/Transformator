using NUnit.Framework;

namespace Transformator.UnitTests
{
    [TestFixture]
    public class TransformationConfigurationTests
    {
        [Test]
        public void Clone_ReturnClonedConfiguration()
        {
            var config = new TransformationConfiguration { InstanceFactory = _ => null };

            var result = config.Clone();

            Assert.IsNotNull(result);
            Assert.AreEqual(config.InstanceFactory, result.InstanceFactory);
            config.InstanceFactory = _ => new object();
            Assert.AreNotEqual(config.InstanceFactory, result.InstanceFactory);
        }
    }
}