using System;
using System.Collections.Generic;
using GiantTextFileSorter.Common;

namespace GiantTextFileSorter.Sorter.Comparers
{
    public class FileLineComparer : IComparer<FileLine>
    {
        public int Compare(FileLine first, FileLine second)
        {
            // if (first.Equals(second))
            // {
            //     return 0;
            // }
            //
            return first.String == second.String
                ? first.Number.CompareTo(second.Number)
                : string.Compare(first.String, second.String, StringComparison.Ordinal);

            // 415. Apple
            // 30432. Something something something
            // 1. Apple
            // 32. Cherry is the best
            // 2. Banana is yellow


            // A signed integer that indicates the relative values of x and y:
            // - If less than 0, x is less than y.
            // - If 0, x equals y.
            // - If greater than 0, x is greater than y.

        }
    }
}