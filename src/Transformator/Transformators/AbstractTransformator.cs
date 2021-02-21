using System.Collections.Generic;

namespace Transformator.Transformators
{
    public abstract class AbstractTransformator<TSource, TDestination> : AbstractTransformation<TSource, TDestination>
    {
        protected AbstractTransformator(TransformationBuilder<TSource, TDestination> builder = null)
        {
            AttachTo(builder);
        }

        protected IList<AbstractTransformation<TSource, TDestination>> GetTransformations()
        {
            return Builder?.Transformations ?? new List<AbstractTransformation<TSource, TDestination>>();
        }
    }
}