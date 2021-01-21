using System.Collections.Generic;
using Transformator.Interfaces;

namespace Transformator
{
    public static class TransformatorExtensions
    {
        public static TDestination Transform<TSource, TDestination>(this ITransformator<TSource, TDestination> transformator, TSource source,
                TransformationContext context = null)
        {
            return transformator.Transform(source, context: context);
        }

        public static IEnumerable<TDestination> TransformMulti<TSource, TDestination>(this IMultiTransformator<TSource, TDestination> transformator,
                TSource source, TransformationContext context = null)
        {
            return transformator.TransformMulti(source, context: context);
        }
    }
}