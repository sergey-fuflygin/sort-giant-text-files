using System;
using System.Collections.Generic;
using GiantTextFileSorter.Common.Models;

namespace GiantTextFileSorter.Sorter.Comparers
{
    public class FileLineComparer : IComparer<FileLine>
    {
        public int Compare(FileLine first, FileLine second)
        {
            return first.String == second.String
                ? first.Number.CompareTo(second.Number)
                : string.Compare(first.String, second.String, StringComparison.Ordinal);
        }
    }
}