using System.Collections.Generic;
using Transformator.Models;

namespace Transformator.Interfaces
{
    /// <summary>
    /// Transforms a single source object to multiple destination objects.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    public interface IMultiTransformator<TSource, TDestination> : ITransformator<TSource, TDestination>
    {
        /// <summary>Do the transformation.</summary>
        /// <param name="source">Source instance.</param>
        /// <param name="initialDestination">Optional initial destination instance.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>0 or many transformation result's objects.</returns>
        IEnumerable<TDestination> TransformMulti(TSource source, TDestination initialDestination = default, TransformationContext context = null);
    }
}