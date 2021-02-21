using Rumo.Transformator.Models;

namespace Rumo.Transformator.Interfaces
{
    /// <summary>
    /// Base class for an abstract transformation step or flow.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    public interface IAbstractTransformation<TSource, TDestination>
    {
        /// <summary>Transform the single source object to a single destionation object.</summary>
        /// <param name="source">Source object to transform.</param>
        /// <param name="destination">Current destination object to transform the source object to. Could be recreated and returned from this method.</param>
        /// <param name="context">Transformation additional custom details, specific to the particular transformation flow.</param>
        /// <returns></returns>
        TDestination Transform(TSource source, TDestination destination = default, TransformationContext context = null);
    }
}