using Transformator.Helpers;
using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator
{
    /// <summary>
    /// Abstracts the transformation of source to destination data types.
    /// </summary>
    /// <typeparam name="TSource">Source data type.</typeparam>
    /// <typeparam name="TDestination">Destination data type.</typeparam>
    /// <seealso cref="Transformator.Interfaces.IAbstractTransformation{TSource, TDestination}" />
    public abstract class AbstractTransformation<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
        /// <summary>The transformation builder that was used to build the transformation flow that this transformation is part of.</summary>
        protected TransformationBuilder<TSource, TDestination> Builder { get; private set; }

        /// <summary>Whether this transformation generates an isolated result by creating new destination instance.</summary>
        /// <value><c>true</c> new destination instance is created and returned; otherwise, <c>false</c> and an existing destination instance is used
        /// upon the transformation.</value>
        public bool IsIsolatedResult { get; set; }

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

        /// <summary>Try to resolve the destination instance.</summary>
        /// <param name="context">Transformation context.</param>
        /// <param name="destination">Existing destination instance or NULL.</param>
        /// <returns>Destination instance or NULL if some of transformers will create it manually.</returns>
        protected internal virtual TDestination GetDestinationInstance(TransformationContext context, TDestination destination)
        {
            if (destination == null || IsIsolatedResult)
                return CreateDestinationInstance(context);

            return destination;
        }

        TDestination CreateDestinationInstance(TransformationContext context)
        {
            if (Builder.InitialDestinationFactory != null)
                return Builder.InitialDestinationFactory(context);
            if (Builder.Configuration.AutoCreateDestination)
                return Builder.Configuration.CreateInstanceSafe<TDestination>();

            return default;
        }
    }
}