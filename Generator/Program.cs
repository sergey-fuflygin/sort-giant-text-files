using System;

namespace GiantTextFileSorter.Generator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var t = new GiantTextFileGenerator("random.txt", 1000);
            t.Generate();
        }
    }
}