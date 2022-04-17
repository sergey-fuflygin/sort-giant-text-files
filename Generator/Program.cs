using System;
using System.Diagnostics;

namespace GiantTextFileSorter.Generator
{
    internal static class Program
    {
        private static void Main(string[] args) 
        {
            var watch = new Stopwatch();
            
            watch.Start();
            
            var textFileGenerator = new GiantTextFileGenerator("random.txt", (long)1024 * 1024 * 1024 * 2); // 1 GB
            textFileGenerator.Generate();
            
            watch.Stop();
            
            Console.WriteLine($"Generation done, took {TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)}");
        }
    }
}