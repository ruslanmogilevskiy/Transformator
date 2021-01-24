using System.Collections.Generic;
using System.Linq;
using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator.Transformators
{
    public class MultiResultTransformator<TSource, TDestination> : AbstractTransformator<TSource, TDestination>,
        IMultiTransformator<TSource, TDestination>
    {
        public MultiResultTransformator(TransformationBuilder<TSource, TDestination> builder) : base(builder)
        {
        }

        public override TDestination Transform(TSource source, TDestination initialDestination, TransformationContext context)
        {
            return TransformMulti(source, initialDestination, context)
                .LastOrDefault();
        }

        public IEnumerable<TDestination> TransformMulti(TSource source, TDestination initialDestination, TransformationContext context)
        {
            // Note: used in case when the transformations list has list of (non-isolated) transformers and then 1-N of isolated transformers.
            // Note: This flag is used to DO NOT return the same destination at the end if it already was sent prior to the isolated transformers.
            var isDestinationWasAlreadyReturned = false;
            // Note: Flag - whether any non-isolated transformer already run. Used if an isolated transformer will run so we need to return already 
            // transformed destination but there were no transformed run (the destination was just instantiated), so don`t yield the destination.
            var isAnyTransformerRun = false;
            var isNullResultReceived = false;
            TDestination destination = default;

            foreach (var transformer in GetTransformations())
            {
                // new isolated branch is the next transformation: return what we already transformed, if so.
                if (transformer.IsIsolatedResult)
                {
                    if (destination != null && !isDestinationWasAlreadyReturned && isAnyTransformerRun && !isNullResultReceived)
                    {
                        yield return destination;
                        isDestinationWasAlreadyReturned = true;
                    }
                }
                else
                {
                    // Note: skip all further non-isolated transformers if we previously got NULL result.
                    if(isNullResultReceived)
                        continue;
                }

                // Note: create or get the destination instance.
                destination ??= GetDestinationInstance(context, initialDestination);

                var result = transformer.Transform(source, destination, context);

                // Note: if some transformer returns NULL we end the transformation for all non-isolated transformers.
                // It`s used to stop (by some condition) the transformation where it is.
                if (result == null)
                {
                    if (!transformer.IsIsolatedResult)
                    {
                        isNullResultReceived = true;
                    }

                    // Note: for isolated result just skip it.
                    continue;
                }


                if (transformer.IsIsolatedResult)
                    yield return result;
                else
                {
                    destination = result;
                    isDestinationWasAlreadyReturned = false;
                    isAnyTransformerRun = true;
                }
            }

            if (!isDestinationWasAlreadyReturned && !isNullResultReceived && destination != null)
                yield return destination;
        }
    }
}