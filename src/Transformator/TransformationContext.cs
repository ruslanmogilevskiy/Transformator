using Transformator.Interfaces;

namespace Transformator
{
    /// <summary>
    /// Used to traverse the transformation context (data) across different transformations.
    /// </summary>
    /// <remarks>Inherit it and pass to <see cref="IAbstractTransformation{TSource,TDestination}.Transform"/></remarks>
    public abstract class TransformationContext
    {
    }
}