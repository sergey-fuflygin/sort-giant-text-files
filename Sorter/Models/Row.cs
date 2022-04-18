using GiantTextFileSorter.Common.Models;

namespace GiantTextFileSorter.Sorter.Models
{
    internal readonly struct Row
    {
        public FileLine FileLine { get; }
        public int StreamReader { get; }

        public Row(FileLine fileLine, int streamReader)
        {
            FileLine = fileLine;
            StreamReader = streamReader;
        }
    }
}