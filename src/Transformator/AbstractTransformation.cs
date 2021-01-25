using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator
{
    public abstract class AbstractTransformation<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
        protected TransformationBuilder<TSource, TDestination> Builder { get; private set; }

        public bool IsIsolatedResult { get; set; }

        internal void AttachTo(TransformationBuilder<TSource, TDestination> builder)
        {
            Builder = builder;
        }

        public abstract TDestination Transform(TSource source, TDestination destination, TransformationContext context);

        protected internal virtual TDestination GetDestinationInstance(TransformationContext context, TDestination destination)
        {
            if (destination == null)
                return CreateDestinationInstance(context);
            return IsIsolatedResult ? CreateDestinationInstance(context) : destination;
        }

        TDestination CreateDestinationInstance(TransformationContext context)
        {
            return Builder.InitialInstanceFactory != null ? Builder.InitialInstanceFactory(context) : default;
        }
    }
}