using System.Collections.Generic;
using Transformator.Interfaces;

namespace Transformator.Transformators
{
    public abstract class AbstractTransformator<TSource, TDestination> : AbstractTransformation<TSource, TDestination>
    {
        protected AbstractTransformator(TransformationBuilder<TSource, TDestination> builder)
        {
            AttachTo(builder);
        }

        protected IList<IAbstractTransformation<TSource, TDestination>> GetTransformations()
        {
            return Builder?.Transformations ?? new List<IAbstractTransformation<TSource, TDestination>>();
        }
    }
}