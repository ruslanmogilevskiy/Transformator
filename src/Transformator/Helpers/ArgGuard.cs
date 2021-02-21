using System;

namespace Transformator.Helpers
{
    /// <summary>
    /// Asserts on arguments.
    /// </summary>
    public static class ArgGuard
    {
        /// <summary>Checks that the argument is not NULL and throws an exception if so.</summary>
        /// <param name="arg">Argument value.</param>
        /// <param name="argName">Argument name that's used in thrown exception.</param>
        /// <exception cref="ArgumentException">Thrown is passed argument is NULL.</exception>
        public static void NotNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }
    }
}