using System.Collections.Generic;

namespace QLESS.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static ICollection<T> AddRange<T>(this ICollection<T> source, ICollection<T> collection)
        {
            foreach (var c in collection)
            {
                source.Add(c);
            }

            return source;
        }
    }
}
