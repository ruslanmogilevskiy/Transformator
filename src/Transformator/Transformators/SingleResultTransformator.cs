using Rumo.Transformator.Interfaces;
using Rumo.Transformator.Models;

namespace Rumo.Transformator.Transformators
{
    /// <summary>
    /// Does the transformation and returns single transformation result.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    /// <seealso cref="Transformator.Transformators.AbstractTransformator{TSource, TDestination}" />
    /// <seealso cref="Transformator.Interfaces.ITransformator{TSource, TDestination}" />
    public class SingleResultTransformator<TSource, TDestination> : AbstractTransformator<TSource, TDestination>,
        ITransformator<TSource, TDestination>
    {
        public SingleResultTransformator(TransformationBuilder<TSource, TDestination> builder) : base(builder)
        {
        }

        /// <inheritdoc/>
        public override TDestination Transform(TSource source, TDestination initialDestination, TransformationContext context)
        {
            var destination = GetDestinationInstance(context, initialDestination);
            foreach (var transformer in GetTransformations())
            {
                destination = transformer.Transform(source, destination, context);
                if (destination == null)
                    return default;
            }

            return destination;
        }
    }
}