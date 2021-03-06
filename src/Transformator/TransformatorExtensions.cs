﻿using System.Collections.Generic;
using Rumo.Transformator.Interfaces;
using Rumo.Transformator.Models;

namespace Rumo.Transformator
{
    /// <summary>
    /// Transformation extensions.
    /// </summary>
    public static class TransformatorExtensions
    {
        /// <summary>Transforms the specified source instance into single destination instance without specifying the initial destination instance.</summary>
        /// <typeparam name="TSource">Source data type to transform from.</typeparam>
        /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
        /// <param name="transformator">Transformator to apply the extension to.</param>
        /// <param name="source">Source instance to transform.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>Single destination instance.</returns>
        public static TDestination Transform<TSource, TDestination>(this ITransformator<TSource, TDestination> transformator, TSource source,
            TransformationContext context = null)
        {
            return transformator.Transform(source, context: context);
        }

        /// <summary>Transforms the specified source instance into multiple destination instances without specifying the initial destination instance.</summary>
        /// <typeparam name="TSource">Source data type to transform from.</typeparam>
        /// <typeparam name="TDestination">Destination data type to transform to.</typeparam>
        /// <param name="transformator">Transformator to apply the extension to.</param>
        /// <param name="source">Source instance to transform.</param>
        /// <param name="context">Transformation context.</param>
        /// <returns>Single destination instance.</returns>
        public static IEnumerable<TDestination> TransformMulti<TSource, TDestination>(this IMultiTransformator<TSource, TDestination> transformator,
            TSource source, TransformationContext context = null)
        {
            return transformator.TransformMulti(source, context: context);
        }
    }
}