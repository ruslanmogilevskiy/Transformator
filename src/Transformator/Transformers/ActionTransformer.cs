using System;
using Transformator.Models;

namespace Transformator.Transformers
{
    /// <summary>
    /// Transformer that executes an action on transformation.
    /// </summary>
    /// <typeparam name="TSource">Source data type.</typeparam>
    /// <typeparam name="TDestination">Destination data type.</typeparam>
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