using System;
using System.Diagnostics.CodeAnalysis;
using Rumo.Transformator.Helpers;

namespace Rumo.Transformator.Configuration
{
    /// <summary>
    /// Global configuration and settings that're applied to all created transformations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TransformationConfiguration
    {
        static TransformationConfiguration _defaultConfiguration = new();

        /// <summary>
        /// Default transformation configuration for all transformations created via <see cref="Transformation.For" />.
        /// </summary>
        public static TransformationConfiguration Default
        {
            get => _defaultConfiguration;
            set
            {
                ArgGuard.NotNull(value, "value");
                _defaultConfiguration = value;
            }
        }

        /// <summary>Abstracts the instantiation of required types within the transformation flow.</summary>
        /// <remarks>Could be used to connect transformation flows with used dependency injection containers, etc.</remarks>
        public Func<Type, object> InstanceFactory { get; set; }

        /// <summary>Whether to instantiate the initial destination transformation instance automatically prior to executing any transformation step.
        /// </summary>
        /// <remarks>By default, it's true.</remarks>
        public bool AutoCreateDestination { get; set; } = true;

        /// <summary>Whether to created a new initial destination instance when processing an isolated transformer.</summary>
        /// <remarks>If <c>false</c>, then the previously created (if so) destination object will be directly passed into an isolated transfomer which could
        /// lead to returning the same destination object from the multi-transformator if the transfomer returns the same received destination object out.
        /// <c>true</c> by default.</remarks>
        public bool IsolateInitialDestination { get; set; } = true;

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