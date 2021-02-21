using FakeItEasy;

namespace Rumo.Transformator.UnitTests.TestHelpers
{
    static class TestExtensions
    {
        public static T AsPartial<T>(this T fake)
        {
            A.CallTo(fake)
                .CallsBaseMethod();
            return fake;
        }

        public static string GetArgumentNullExceptionMessage(this string argName)
        {
            return $"Value cannot be null.\r\nParameter name: {argName}";
        }
    }
}