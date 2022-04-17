using GiantTextFileSorter.Common;

namespace GiantTextFileSorter.Sorter
{
    internal readonly struct Row
    {
        public Row(FileLine fileLine, int streamReader)
        {
            FileLine = fileLine;
            StreamReader = streamReader;
        }

        public FileLine FileLine { get; }
        public int StreamReader { get; }
    }
}