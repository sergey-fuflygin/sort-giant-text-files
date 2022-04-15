using System;
using System.Diagnostics;

namespace GiantTextFileSorter.Sorter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            
            var giantTextFileSorter = new GiantTextFileSorter("random.txt", "sorted.txt");
            giantTextFileSorter.Sort();
            
            watch.Stop();
            Console.WriteLine($"Sorting completed, took {watch.Elapsed}");
        }
    }
}