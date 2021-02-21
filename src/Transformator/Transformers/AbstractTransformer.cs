using System.Diagnostics.CodeAnalysis;
using Transformator.Models;

namespace Transformator.Transformers
{
    /// <summary>
    /// Base transformer that uses <see cref="TransformationContext"/> as a transformation context.
    /// </summary>
    /// <typeparam name="TSource">Source data type to transform from.</typeparam>
    /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class AbstractTransformer<TSource, TDestination> : TypedAbstractTransformer<TSource, TDestination, TransformationContext>
    {
    }
}