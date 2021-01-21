namespace Transformator.Transformers
{
    public enum TransformAction
    {
        /// <summary>Do the transformation.</summary>
        Transform,

        /// <summary>Return the received destination instance, without transformation.</summary>
        PassThrough,

        /// <summary>Return NULL so break the transformation chain.</summary>
        BreakTransformation
    }
}