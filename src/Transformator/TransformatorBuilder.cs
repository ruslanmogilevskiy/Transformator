namespace Transformator
{
    public static class TransformatorBuilder
    {
        public static TransformationBuilder<TSource, TDestination> For<TSource, TDestination>()
        {
            return new();
        }
    }
}