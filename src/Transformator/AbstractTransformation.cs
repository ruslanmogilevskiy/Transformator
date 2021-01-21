using Transformator.Interfaces;

namespace Transformator
{
    public abstract class AbstractTransformation<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
        internal TransformationBuilder<TSource, TDestination> Builder { get; private set; }

        public bool IsIsolatedResult { get; set; }

        internal void AttachTo(TransformationBuilder<TSource, TDestination> builder)
        {
            Builder = builder;
        }

        public abstract TDestination Transform(TSource source, TDestination destination, TransformationContext context);

        protected virtual TDestination GetDestinationInstance(TransformationContext context, TDestination destination)
        {
            if (destination == null)
                return CreateDestinationInstance(context);
            return IsIsolatedResult ? CreateDestinationInstance(context) : destination;
        }

        TDestination CreateDestinationInstance(TransformationContext context)
        {
            return Builder.InitialInstanceProvider != null ? Builder.InitialInstanceProvider(context) : default;
        }
    }
}