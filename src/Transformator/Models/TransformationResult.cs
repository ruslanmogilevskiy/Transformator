using System.Collections.Generic;
using System.Linq;

namespace Transformator.Models
{
    class TransformationResult<TDestination>
    {
        public IList<TDestination> Destinations { get; } = new List<TDestination>();

        public bool HasNotNullDestination()
        {
            return Destinations.Any(d => d != null);
        }

        public bool IsEmpty()
        {
            return !Destinations.Any();
        }

        public void Add(TDestination item)
        {
            Destinations.Add(item);
        }

        public TDestination GetSingleOrDefault()
        {
            return Destinations.Count == 1 ? Destinations[0] : default;
        }

        public void RemoveAt(int index)
        {
            Destinations.RemoveAt(index);
        }

        public void Insert(int index, TDestination item)
        {
            Destinations.Insert(index, item);
        }
    }
}