using System;

namespace Transformator.Transformers
{
    public class ActionTransformer<TSource, TDestination> : AbstractTransformer<TSource, TDestination>
    {
        readonly Func<TSource, TDestination, TransformationContext, TDestination> _action;

        public ActionTransformer(Func<TSource, TDestination, TransformationContext, TDestination> action)
        {
            _action = action;
        }

        protected override TDestination DoTransform(TSource source, TDestination destination, TransformationContext context)
        {
            return _action(source, destination, context);
        }
    }
}