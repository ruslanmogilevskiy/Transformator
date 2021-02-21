using System;
using Transformator.Models;

namespace Transformator.Transformers
{
    /// <summary>
    /// Evaluates the passed condition and do the transformation only if it <c>true</c>.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    internal class ConditionalTransformer<TSource, TDestination> : AbstractTransformer<TSource, TDestination>
    {
        readonly Func<TSource, TDestination, TransformationContext, bool> _condition;
        readonly Func<TSource, TDestination, TransformationContext, TDestination> _action;

        public ConditionalTransformer(Func<TSource, TDestination, TransformationContext, bool> condition,
            Func<TSource, TDestination, TransformationContext, TDestination> action, bool isolatedResult = false)
        {
            IsIsolatedResult = isolatedResult;
            _condition = condition;
            _action = action;
        }

        protected override TDestination DoTransform(TSource source, TDestination destination, TransformationContext context)
        {
            if (_condition(source, destination, context))
            {
                destination = GetDestinationInstance(context, destination);
                destination = _action(source, destination, context);
            }

            return destination;
        }
    }
}