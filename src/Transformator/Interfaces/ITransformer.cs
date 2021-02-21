namespace Rumo.Transformator.Interfaces
{
    /// <summary>
    /// Single piece of an object transformation. Transforms an object from one state to another.
    /// </summary>
    /// <typeparam name="TSource">Source transformation object's type.</typeparam>
    /// <typeparam name="TDestination">Destination transformation object's type.</typeparam>
    public interface ITransformer<TSource, TDestination> : IAbstractTransformation<TSource, TDestination>
    {
    }
}