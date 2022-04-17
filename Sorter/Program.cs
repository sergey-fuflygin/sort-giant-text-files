using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using GiantTextFileSorter.Sorter.Extensions;

namespace GiantTextFileSorter.Sorter
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            
            var unsortedFile = File.OpenRead("random.txt");
            var targetFile = File.OpenWrite("sorted.txt");
            var sorter = new GiantTextFileSorter();
            
            await sorter.Sort(unsortedFile, targetFile);
            
            watch.Stop();
            Console.WriteLine($"{new FileInfo("random.txt").Length.ToBytes()} file sorted in {watch.Elapsed}.");
        }
    }
}