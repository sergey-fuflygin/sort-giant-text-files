using System;
using System.Collections.Generic;

namespace GiantTextFileSorter.Sorter
{
    public class ExternalMergeSorterOptions
    {
        public ExternalMergeSorterOptions()
        {
            Split = new ExternalMergeSortSplitOptions();
            Sort = new ExternalMergeSortSortOptions();
            Merge = new ExternalMergeSortMergeOptions();
        }

        public static string 
            FileLocation => "";
        private ExternalMergeSortSplitOptions Split { get; }
        public ExternalMergeSortSortOptions Sort { get; }
        private ExternalMergeSortMergeOptions Merge { get; }
    }

    public class ExternalMergeSortSplitOptions
    {
        /// <summary>
        /// Size of unsorted file (chunk) (in bytes)
        /// </summary>
        public static int FileSize => 1024 * 1024 * 1024;

        public static char NewLineSeparator => '\n';
    }

    public class ExternalMergeSortSortOptions
    {
        public IComparer<string> Comparer { get; } = Comparer<string>.Default;
        public static int InputBufferSize => 65536;
        public static int OutputBufferSize => 65536;
        public static IProgress<double> ProgressHandler => null!;
    }

    public class ExternalMergeSortMergeOptions
    {
        /// <summary>
        /// How many files we will process per run
        /// </summary>
        public static int FilesPerRun => 10;

        /// <summary>
        /// Buffer size (in bytes) for input StreamReaders
        /// </summary>
        public static int InputBufferSize => 65536;

        /// <summary>
        /// Buffer size (in bytes) for output StreamWriter
        /// </summary>
        public static int OutputBufferSize => 65536;

        public static IProgress<double> ProgressHandler => null!;
    }
}