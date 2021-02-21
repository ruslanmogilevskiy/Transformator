using System.Collections.Generic;
using System.Linq;
using Rumo.Transformator.Interfaces;
using Rumo.Transformator.Models;

namespace Rumo.Transformator.Transformators
{
    /// <summary>Transformations single source to multiple destination objects.</summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    public class MultiResultTransformator<TSource, TDestination> : AbstractTransformator<TSource, TDestination>,
        IMultiTransformator<TSource, TDestination>
    {
        public MultiResultTransformator(TransformationBuilder<TSource, TDestination> builder) : base(builder)
        {
        }

        /// <inheritdoc/>
        public override TDestination Transform(TSource source, TDestination initialDestination, TransformationContext context)
        {
            return TransformMulti(source, initialDestination, context)
                .LastOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<TDestination> TransformMulti(TSource source, TDestination initialDestination, TransformationContext context)
        {
            // Note: used in case when the transformations list has list of (non-isolated) transformers and then 1-N of isolated transformers.
            // Note: This flag is used to DO NOT return the same destination at the end if it already was sent prior to the isolated transformers.
            var isDestinationAlreadyReturned = false;
            var isNullResultReceived = false;
            TransformationResult<TDestination> results = new();

            foreach (var transformer in GetTransformations())
            {
                // Note: process isolated transformation.
                if (transformer.IsIsolatedResult)
                {
                    // Note: yield already transformed non-NULL non-isolated results.
                    if (!isDestinationAlreadyReturned && results.HasNotNullDestination())
                    {
                        foreach (var nonIsolatedDestination in results.Destinations.Where(d => d != null))
                        {
                            yield return nonIsolatedDestination;
                        }

                        isDestinationAlreadyReturned = true;
                    }

                    // Note: process an isolated transformer separately (w/o iterating all existing destinations)
                    // Note: into an isolated transformer we pass either a single existing destination object or create a new one if here`re many of them exist.
                    var isolatedDestination = transformer.GetDestinationInstance(context, results.GetSingleOrDefault() ?? initialDestination);
                    foreach (var result in TransformAndGetResults(transformer, source, isolatedDestination, context))
                    {
                        if(result==null)
                            break;
                        yield return result;
                    }

                    continue;
                }

                // Note: skip all further non-isolated transformers if we previously got NULL result.
                if (isNullResultReceived)
                    continue;

                // Note: create initial destination instance.
                if (results.IsEmpty())
                    results.Add(transformer.GetDestinationInstance(context, initialDestination));

                // Note: process NON-isolated transformation.
                for (var i = 0; i < results.Destinations.Count; i++)
                {
                    var destination = results.Destinations[i];
                    var transformationResults = TransformAndGetResults(transformer, source, destination, context)?.ToList();
                    // Note: if transformer returned empty result we remove current destination and proceed with the next destination.
                    if (transformationResults == null || !transformationResults.Any())
                    {
                        results.RemoveAt(i--);
                        continue;
                    }

                    // Note: remove current destination to replace it with received transformation results.
                    results.RemoveAt(i--);

                    foreach (var result in transformationResults)
                    {
                        // Note: if some transformer returns NULL we end the transformation for all non-isolated transformers.
                        // It`s used to stop (by some condition) the transformation where it is.
                        if (result == null)
                        {
                            isNullResultReceived = true;
                            break;
                        }
                        // Note: register new result.
                        results.Insert(++i, result);

                        isDestinationAlreadyReturned = false;
                    }

                    // Note: if above we received NULL from the transformer skip all other destinations (break the transformation).
                    if (isNullResultReceived)
                        break;
                }

                // Note: if current transformer returned empty result for all existing destinations we treat this like NULL result was received and break the transformation.
                if (results.IsEmpty())
                    isNullResultReceived = true;
            }

            if (!isDestinationAlreadyReturned && results.HasNotNullDestination())
                foreach (var nonIsolatedDestination in results.Destinations.Where(d => d != null))
                {
                    yield return nonIsolatedDestination;
                }
        }

        protected virtual IEnumerable<TDestination> TransformAndGetResults(IAbstractTransformation<TSource, TDestination> transformer, TSource source,
            TDestination destination, TransformationContext context)
        {
            if (transformer is IMultiTransformer<TSource, TDestination> multiTransformer)
                return multiTransformer.TransformMulti(source, destination, context);
            return new[] { transformer.Transform(source, destination, context) };
        }
    }
}