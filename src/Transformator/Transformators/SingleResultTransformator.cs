using Transformator.Interfaces;
using Transformator.Models;

namespace Transformator.Transformators
{
    public class SingleResultTransformator<TSource, TDestination> : AbstractTransformator<TSource, TDestination>,
        ITransformator<TSource, TDestination>
    {
        public SingleResultTransformator(TransformationBuilder<TSource, TDestination> builder) : base(builder)
        {
        }

        public override TDestination Transform(TSource source, TDestination initialDestination, TransformationContext context)
        {
            var destination = GetDestinationInstance(context, initialDestination);
            foreach (var transformer in GetTransformations())
            {
                var result = transformer.Transform(source, destination, context);
                // if some transformer returns NULL we end the transformation and return the current destination state.
                // it`s used to stop the transformation by some condition.
                if (result == null)
                    break;

                destination = result;
            }

            return destination;
        }
    }
}