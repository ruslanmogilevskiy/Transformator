namespace Rumo.Transformator.Interfaces
{
    /// <summary>
    /// Transforms an object via chain of registered transformations.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    public interface ITransformator<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
    }
}