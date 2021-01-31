using System;
using System.Diagnostics.CodeAnalysis;

namespace Transformator
{
    /// <summary>
    /// Transformation configuration and settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TransformationConfiguration
    {
        /// <summary>
        /// The transformation configuration that's used as a basis configuration for all transformations created with <see cref="Transformation.For" />.
        /// </summary>
        public static TransformationConfiguration Default { get; set; }

        /// <summary>Abstracts the instantiation of required types within the transformation flow.</summary>
        /// <remarks>Could be used to connect transformation flows with used dependency injection containers, etc.</remarks>
        public Func<Type, object> InstanceFactory { get; set; }

        /// <summary>Whether to instantiate the initial destination transformation instance automatically prior to executing any transformation step.
        /// </summary>
        /// <remarks>By default, it's true.</remarks>
        public bool AutoCreateDestination { get; set; } = true;

        /// <summary>Clones this instance.</summary>
        public TransformationConfiguration Clone()
        {
            return new()
            {
                AutoCreateDestination = AutoCreateDestination,
                InstanceFactory = InstanceFactory
            };
        }
    }
}