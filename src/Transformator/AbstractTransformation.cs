using Transformator.Helpers;
using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator
{
    /// <summary>
    /// Abstracts the transformation of source to destination data types.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    /// <seealso cref="Transformator.Interfaces.IAbstractTransformation{TSource, TDestination}" />
    public abstract class AbstractTransformation<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
        /// <summary>The transformation builder that was used to build the transformation flow that this transformation is part of.</summary>
        protected TransformationBuilder<TSource, TDestination> Builder { get; private set; }

        /// <inheritdoc/>
        public bool IsIsolatedResult { get; set; }

        /// <inheritdoc/>
        public bool KeepInitialDestination { get; set; }

        internal void AttachTo(TransformationBuilder<TSource, TDestination> builder)
        {
            Builder = builder;
        }

        /// <summary>Transforms the passed source object into the destination object.</summary>
        /// <param name="source">Source data instance. Use it to compose the <paramref name="destination" /> instance.</param>
        /// <param name="destination">Destination data instance that will be returned as a transformation flow's result. Could be replaced with a new
        /// instance or NULL.</param>
        /// <param name="context">Transformation context that could store additional settings specific for this kind of transformation.</param>
        /// <returns>Transformation's result. Usually it's the same received destination object parameter.</returns>
        public abstract TDestination Transform(TSource source, TDestination destination, TransformationContext context);

        /// <summary>Check whether the transformation has an isolated destination.</summary>
        bool IsIsolatedDestination()
        {
            return IsIsolatedResult && !KeepInitialDestination;
        }

        /// <summary>Try to resolve the destination instance.</summary>
        /// <param name="context">Transformation context.</param>
        /// <param name="destination">Existing destination instance or NULL.</param>
        /// <returns>Destination instance or NULL if some of transformers will create it manually.</returns>
        protected internal virtual TDestination GetDestinationInstance(TransformationContext context, TDestination destination)
        {
            // Note: if destination object is isolated just created new instance. Otherwise, reuse the existing instance or created new.

            // keep existing destination for transformers with NON-isolated destinations.
            if (destination != null && !IsIsolatedDestination())
                return destination;
            if (Builder?.InitialDestinationFactory != null)
                return Builder.InitialDestinationFactory(context);
            if (Builder?.Configuration?.AutoCreateDestination == true)
                return Builder.Configuration.CreateInstanceSafe<TDestination>();

            return default;
        }
    }
}