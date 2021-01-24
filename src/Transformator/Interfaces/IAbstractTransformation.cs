using Transformator.Models;

namespace Transformator.Interfaces
{
    public interface IAbstractTransformation<TSource, TDestination>
    {
        bool IsIsolatedResult { get; set; }
    
        TDestination Transform(TSource source, TDestination destination = default, TransformationContext context = null);
    }
}