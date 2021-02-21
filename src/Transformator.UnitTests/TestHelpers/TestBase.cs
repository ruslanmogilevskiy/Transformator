using NUnit.Framework;
using Rumo.Transformator.Configuration;

namespace Rumo.Transformator.UnitTests.TestHelpers
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