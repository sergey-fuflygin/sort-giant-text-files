using System;
using System.Collections.Generic;
using GiantTextFileSorter.Common;

namespace GiantTextFileSorter.Sorter.Comparers
{
    public class TextLineComparer : IComparer<string>
    {
        public int Compare(string first, string second)
        {
            if (first == null)
            {
                return second == null ? 0 : -1;
            }            
            
            if (second == null)
            {
                return 1;
            }

            var (firstNumber, firstString) = GetNumberAndString(first.AsSpan());
            var (secondNumber, secondString) = GetNumberAndString(second.AsSpan());
            
            return firstString == secondString
                ? firstNumber.CompareTo(secondNumber)
                : string.Compare(firstString, secondString, StringComparison.Ordinal);

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

        private static (int firstNumber, string firstString) GetNumberAndString(ReadOnlySpan<char> span)
        {
            var dotPosition = span.IndexOf(". ");

            var number = int.Parse(span[..dotPosition]);
            var @string = span[(dotPosition + 2)..].ToString();
            
            return (number, @string);
        }
    }
}