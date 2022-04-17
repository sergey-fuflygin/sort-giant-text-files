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
            
            // var textFileSorter = new GiantTextFileSorter("random.txt", "sorted.txt");
            // textFileSorter.Sort();
            
            var unsortedFile = File.OpenRead("random.txt");
            var targetFile = File.OpenWrite("sorted.txt");
            var sorter = new ExternalMergeSorter();
            
            await sorter.Sort(unsortedFile, targetFile);
            
            watch.Stop();
            
            var currentProcess = Process.GetCurrentProcess();
            var totalBytesOfMemoryUsed = currentProcess.WorkingSet64;
            
            Console.WriteLine($"{new FileInfo("random.txt").Length.ToBytes()} file sorted in {watch.Elapsed}. " +
                              $"{totalBytesOfMemoryUsed.ToBytes()} of memory used.");
        }
    }
}