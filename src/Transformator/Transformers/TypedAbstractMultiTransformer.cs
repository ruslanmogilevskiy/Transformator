using System;
using System.Collections.Generic;
using Rumo.Transformator.Interfaces;
using Rumo.Transformator.Models;

namespace Rumo.Transformator.Transformers
{
    /// <summary>
    /// Base class for transformers from single TSource object to multiple TDestination objects with specific transformation context.
    /// </summary>
    /// <remarks>Notice that the Transform method is not supported and throw an exception.</remarks>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    /// <typeparam name="TContext">Transformation context type.</typeparam>
    /// <seealso cref="Transformator.AbstractTransformation{TSource, TDestination}" />
    /// <seealso cref="Transformator.Interfaces.ITransformer{TSource, TDestination}" />
    public abstract class TypedAbstractMultiTransformer<TSource, TDestination, TContext> : AbstractTransformation<TSource, TDestination>,
        IMultiTransformer<TSource, TDestination> where TContext : TransformationContext
    {
        /// <inheritdoc />
        public override TDestination Transform(TSource source, TDestination destination, TransformationContext context)
        {
            throw new NotSupportedException($"Only multi-transformation is supported via {nameof(TransformMulti)} method");
        }

        /// <inheritdoc />
        public IEnumerable<TDestination> TransformMulti(TSource source, TDestination destination = default,
            TransformationContext context = null)
        {
            switch (CanTransform(source, destination, (TContext) context))
            {
                case TransformAction.Transform:
                    return DoMultiTransform(source, destination, (TContext) context);

                case TransformAction.PassThrough:
                    return new[]{ destination };

                case TransformAction.BreakTransformation:
                default:
                    return new TDestination[0];
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

        /// <summary>Do actual transformation with multiple destination results.</summary>
        /// <param name="source">Source data instance.</param>
        /// <param name="destination">Destination data instance.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>Transformation's result objects.</returns>
        protected abstract IEnumerable<TDestination> DoMultiTransform(TSource source, TDestination destination, TContext context);
    }
}