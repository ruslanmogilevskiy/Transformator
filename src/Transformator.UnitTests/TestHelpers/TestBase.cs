using NUnit.Framework;
using Transformator.Configuration;

namespace Transformator.UnitTests.TestHelpers
{
    public abstract class TestBase
    {
        [SetUp]
        public virtual void Setup()
        {
            TransformationConfiguration.Default = new TransformationConfiguration();
        } 
    }
}