using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator.Transformers
{
    /// <summary>
    /// Base class for transformers from TSource to TDestination data types with specific transformaton context.
    /// </summary>
    /// <typeparam name="TSource">Source data type.</typeparam>
    /// <typeparam name="TDestination">Destination data type.</typeparam>
    /// <typeparam name="TContext">Transformaton context type.</typeparam>
    /// <seealso cref="Transformator.AbstractTransformation{TSource, TDestination}" />
    /// <seealso cref="Transformator.Interfaces.ITransformer{TSource, TDestination}" />
    public abstract class TypedAbstractTransformer<TSource, TDestination, TContext> : AbstractTransformation<TSource, TDestination>,
        ITransformer<TSource, TDestination> where TContext : TransformationContext
    {
        /// <inheritdoc />
        public override TDestination Transform(TSource source, TDestination destination, TransformationContext context)
        {
            switch (CanTransform(source, destination, (TContext) context))
            {
                case TransformAction.Transform:
                    return DoTransform(source, destination, (TContext) context);

                case TransformAction.PassThrough:
                    return destination;

                case TransformAction.BreakTransformation:
                default:
                    return default;
            }
        }

        /// <summary>Defines whether this transformation be applied.</summary>
        /// <param name="source">Source data instance.</param>
        /// <param name="destination">Destination data instance.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>The transformation action that defines whether this transformation must be executed or skipped. by default, the transformation
        /// will be executed.</returns>
        protected virtual TransformAction CanTransform(TSource source, TDestination destination, TContext context)
        {
            return TransformAction.Transform;
        }

        /// <summary>Do actual transformation.</summary>
        /// <param name="source">Source data instance.</param>
        /// <param name="destination">Destination data instance.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>Transformation's result. Usually it's the same received destination object parameter.</returns>
        protected abstract TDestination DoTransform(TSource source, TDestination destination, TContext context);
    }
}