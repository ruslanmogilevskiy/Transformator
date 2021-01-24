using System;
using Transformator.Models;

namespace Transformator.Transformers
{
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