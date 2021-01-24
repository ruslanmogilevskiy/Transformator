using System;

namespace Transformator.Helpers
{
    public static class ArgGuard
    {
        public static void NotNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentException($"Argument '{argName}' cannot be null");
        }
    }
}