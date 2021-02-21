using System.Collections.Generic;
using Transformator.Models;

namespace Transformator.Interfaces
{
    /// <summary>
    /// Transforms a single source object to multiple destination objects.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    public interface IMultiTransformer<TSource, TDestination> : ITransformer<TSource, TDestination>
    {
        /// <summary>Transforms single source object to multiple destination objects.</summary>
        /// <remarks>Each returned destination object will form a new transformation tree, with own destination object which will pass through the next
        /// transformers in the chain. I.e. the passed <paramref name="initialDestination"/> object will be replaced with multiple objects returned by
        /// this method.
        /// None objects could be returned as well which will effectively remove the passed <paramref name="initialDestination"/> from whole
        /// transformation result set.</remarks>
        /// <param name="source">Source object to transform.</param>
        /// <param name="initialDestination">Initial destination object.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>0 or many transformation result's objects.</returns>
        IEnumerable<TDestination> TransformMulti(TSource source, TDestination initialDestination = default, TransformationContext context = null);
    }
}