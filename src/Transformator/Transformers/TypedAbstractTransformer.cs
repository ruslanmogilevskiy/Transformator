using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator.Transformers
{
    public abstract class TypedAbstractTransformer<TSource, TDestination, TContext> : AbstractTransformation<TSource, TDestination>,
        ITransformer<TSource, TDestination> where TContext : TransformationContext
    {
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

        protected virtual TransformAction CanTransform(TSource source, TDestination destination, TContext context)
        {
            return TransformAction.Transform;
        }

        protected abstract TDestination DoTransform(TSource source, TDestination destination, TContext context);
    }
}