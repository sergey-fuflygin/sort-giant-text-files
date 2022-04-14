using System;

namespace GiantTextFileSorter.Generator
{
    public struct FileLine
    {
        public int Number { get; internal set; }
        public string String { get; internal set; }

        public FileLine(int number, string @string)
        {
            Number = number;
            String = @string;
        }

        public override string ToString() => $"{Number.ToString()}. {String}";
    }
}