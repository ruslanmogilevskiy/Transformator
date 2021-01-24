using System.Diagnostics.CodeAnalysis;
using Transformator.Interfaces;

namespace Transformator.Models
{
    /// <summary>
    /// Used to traverse the transformation context (data) across different transformations.
    /// </summary>
    /// <remarks>Inherit it and pass to <see cref="IAbstractTransformation{TSource,TDestination}.Transform"/></remarks>
    [ExcludeFromCodeCoverage]
    public abstract class TransformationContext
    {
    }
}