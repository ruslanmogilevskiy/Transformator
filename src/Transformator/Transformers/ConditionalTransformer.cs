using System;
using Transformator.Interfaces;

namespace Transformator.Transformers
{
    internal class ConditionalTransformer<TSource, TDestination> : AbstractTransformer<TSource, TDestination>
    {
        readonly IAbstractTransformation<TSource, TDestination> _transformation;
        readonly Func<TSource, TDestination, TransformationContext, bool> _condition;
        readonly Func<TSource, TDestination, TransformationContext, TDestination> _action;

        public ConditionalTransformer(Func<TSource, TDestination, TransformationContext, bool> condition,
            Func<TSource, TDestination, TransformationContext, TDestination> action, bool isolatedResult = false)
        {
            IsIsolatedResult = isolatedResult;
            _condition = condition;
            _action = action;
        }

        public ConditionalTransformer(Func<TSource, TDestination, TransformationContext, bool> condition, IAbstractTransformation<TSource, TDestination> transformation,
            bool isolatedResult = false)
        {
            IsIsolatedResult = isolatedResult;
            _condition = condition;
            _transformation = transformation;
        }

        protected override TDestination DoTransform(TSource source, TDestination destination, TransformationContext context)
        {
            if (!_condition(source, destination, context))
                return destination;

            destination = GetDestinationInstance(context, destination);
            return _transformation != null ? _transformation.Transform(source, destination, context) : _action(source, destination, context);
        }
    }
}