using System;

namespace Transformator.Helpers
{
    internal static class TransformationExtensions
    {
        public static T CreateInstanceSafe<T>(this TransformationConfiguration configuration)
        {
            if (configuration?.InstanceFactory != null)
                return (T) configuration.InstanceFactory(typeof(T));

            return Activator.CreateInstance<T>();
        }
    }
}