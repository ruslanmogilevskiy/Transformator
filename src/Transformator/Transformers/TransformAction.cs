﻿namespace Rumo.Transformator.Transformers
{
    /// <summary>
    /// Defines what to do at the particular transformation step.
    /// </summary>
    public enum TransformAction
    {
        /// <summary>Apply the transformation.</summary>
        Transform,

        /// <summary>Don't apply the transformation but do not break the transformation flow.</summary>
        PassThrough,

        /// <summary>Break the whole transformation flow.</summary>
        BreakTransformation
    }
}