using FakeItEasy;

namespace Transformator.UnitTests.TestHelpers
{
    static class TestExtensions
    {
        public static T AsPartial<T>(this T fake)
        {
            A.CallTo(fake)
                .CallsBaseMethod();
            return fake;
        }
    }
}