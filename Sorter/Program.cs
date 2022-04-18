using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using GiantTextFileSorter.Common.Extensions;
using GiantTextFileSorter.Sorter.Models;

namespace GiantTextFileSorter.Sorter
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsedAsync(ExecuteSorter);
        }

        private static async Task ExecuteSorter(CommandLineOptions opts)
        {
            var watch = new Stopwatch();
            watch.Start();

            var sourceFileName = opts.SourceFile.Trim();
            var targetFileName = opts.TargetFile.Trim();
            
            var unsortedFile = File.OpenRead(sourceFileName);
            var targetFile = File.OpenWrite(targetFileName);
            var sorter = new GiantTextFileSorter();
            
            await sorter.Sort(unsortedFile, targetFile);
            
            Console.WriteLine($"{new FileInfo(sourceFileName).Length.ToBytes()} file sorted in {watch.Elapsed:m\\:ss}.");
        }
    }
}