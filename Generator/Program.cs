using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using GiantTextFileSorter.Common.Extensions;
using GiantTextFileSorter.Generator.Models;

namespace GiantTextFileSorter.Generator
{
    internal static class Program
    {
        private static void Main(string[] args) 
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(ExecuteGenerator);
        }

        private static void ExecuteGenerator(CommandLineOptions opts)
        {
            var watch = new Stopwatch();
            watch.Start();

            var fileName = opts.FileName.Trim();
            
            Console.WriteLine($"Generating...");

            var textFileGenerator = new GiantTextFileGenerator(fileName, opts.FileSize);
            textFileGenerator.Generate();

            Console.WriteLine($"Generated {new FileInfo(fileName).Length.ToBytes()} file in {watch.Elapsed:m\\:ss}.");
        }
    }
}