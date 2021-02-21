using Transformator.Models;

namespace Transformator.Interfaces
{
    /// <summary>
    /// Base class for an abstract transformation step.
    /// </summary>
    /// <typeparam name="TSource">Source data type.</typeparam>
    /// <typeparam name="TDestination">Destination data type.</typeparam>
    public interface IAbstractTransformation<TSource, TDestination>
    {
        /// <summary>Whether this transformer is an isolated leaf of the transformation tree and produces own destination object that's isolated
        /// from the main transformation tree.</summary>
        bool IsIsolatedResult { get; set; }

        /// <summary>Transform the single source object to a single destionation object.</summary>
        /// <param name="source">Source object to transform.</param>
        /// <param name="destination">Current destination object to transform the source object to. Could be recreated and returned from this method.</param>
        /// <param name="context">Transformation additional custom details, specific to the particular transformation flow.</param>
        /// <returns></returns>
        TDestination Transform(TSource source, TDestination destination = default, TransformationContext context = null);
    }
}