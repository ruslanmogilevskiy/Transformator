using Transformator.Models;

namespace Transformator
{
    /// <summary>
    /// Transformation builders factory.
    /// </summary>
    public static class TransformatorBuilder
    {
        /// <summary>
        /// Transformation default settings that are applied to each transformation builder created via
        /// <see cref="For{TSource,TDestination}" /> method.
        /// </summary>
        public static TransformationSettings DefaultSettings { get; set; } = new();

        /// <summary>Defines a transformation builder to transform from <c>TSource</c> to <c>TDestination</c> data types.</summary>
        /// <typeparam name="TSource">Source data type that the transformation flow will start from and receive it as an input.</typeparam>
        /// <typeparam name="TDestination">Destination data type that the transformation flow must produce in result.</typeparam>
        /// <returns>Transformation builder for specified data types.</returns>
        public static TransformationBuilder<TSource, TDestination> For<TSource, TDestination>()
        {
            return new(DefaultSettings);
        }
    }
}