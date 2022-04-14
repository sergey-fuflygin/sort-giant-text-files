using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace GiantTextFileSorter.Sorter
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var watch = new Stopwatch();
            
            watch.Start();
            
            var giantTextFileSorter = new GiantTextFileSorter("random.txt", "sorted.txt");
            await giantTextFileSorter.SortAsync();
            
            watch.Stop();
            
            Console.WriteLine($"Sort done, took {TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)}");
        }
    }
}