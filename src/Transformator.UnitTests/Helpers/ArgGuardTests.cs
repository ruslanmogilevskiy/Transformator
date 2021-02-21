using System;
using NUnit.Framework;
using Transformator.Helpers;
using Transformator.UnitTests.TestHelpers;

namespace Transformator.UnitTests.Helpers
{
    [TestFixture]
    public class ArgGuardTests
    {
        [Test]
        public void NotNull_PassedArgumentIsNotNull_NoExceptionThrown()
        {
            Exception thrownException=null;
            try
            {
                ArgGuard.NotNull("sdf", "arg1");
            }
            catch (Exception exc)
            {
                thrownException = exc;
            }

            Assert.IsNull(thrownException);
        }

        [Test]
        public void NotNull_PassedArgumentIsNull_ExceptionThrown()
        {
            var thrownException = Assert.Throws<ArgumentNullException>(() => ArgGuard.NotNull(null, "arg2"));

            Assert.IsNotNull(thrownException);
            Assert.AreEqual("arg2".GetArgumentNullExceptionMessage(), thrownException.Message);
        }
    }
}