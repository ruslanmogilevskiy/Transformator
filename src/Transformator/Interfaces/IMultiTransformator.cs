using System.Collections.Generic;
using Transformator.Models;

namespace Transformator.Interfaces
{
    /// <summary>
    /// Transforms an object via chain of registered transformations.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    public interface IMultiTransformator<TSource, TDestination> : ITransformator<TSource, TDestination>
    {
        IEnumerable<TDestination> TransformMulti(TSource source, TDestination initialDestination = default, TransformationContext context = null);
    }
}