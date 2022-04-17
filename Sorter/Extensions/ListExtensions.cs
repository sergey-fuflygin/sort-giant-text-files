using System.Collections.Generic;
using System.Linq;

namespace GiantTextFileSorter.Sorter.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static IReadOnlyList<IReadOnlyList<T>> ChunkBy<T>(this IReadOnlyList<T> source, int chunkSize) 
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }        
    }
}