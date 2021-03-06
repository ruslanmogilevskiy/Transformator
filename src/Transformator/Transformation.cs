﻿using Rumo.Transformator.Configuration;

namespace Rumo.Transformator
{
    /// <summary>
    /// Transformation builders factory.
    /// </summary>
    public static class Transformation
    {
        /// <summary>Defines a transformation builder to transform from <c>TSource</c> to <c>TDestination</c> data types.</summary>
        /// <typeparam name="TSource">Source data type that the transformation flow will start from and receive it as an input.</typeparam>
        /// <typeparam name="TDestination">Destination data type that the transformation flow must produce in result.</typeparam>
        /// <returns>Transformation builder for specified data types.</returns>
        public static TransformationBuilder<TSource, TDestination> For<TSource, TDestination>()
        {
            return new(TransformationConfiguration.Default.Clone());
        }
    }
}