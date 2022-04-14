using System;

namespace GiantTextFileSorter.Common
{
    public struct FileLine : IComparable
    {
        public int Number { get;  set; }
        public string String { get; set; }

        public FileLine(int number, string @string)
        {
            Number = number;
            String = @string;
        }

        public override string ToString() => $"{Number.ToString()}. {String}";
        
        public int CompareTo(object obj)
        {

            return 0;
            // var fileLine = (FileLine)obj;
            // if (obj == null) return 1;
            //
            // if (fileLine != null)
            // {
            //     if (this.ProductId < fileLine.ProductId) return 1;
            //     else
            //         return 0;
            //
            // }
            // else {
            //     return 0;
            // }
        
        }
    }
}