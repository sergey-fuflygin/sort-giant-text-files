using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using GiantTextFileSorter.Common;
using GiantTextFileSorter.Sorter.Comparers;

namespace GiantTextFileSorter.Sorter
{
    public class GiantTextFileSorter
    {
        private readonly string _sourceFileName;
        private readonly string _destinationFileName;
        
        public GiantTextFileSorter(string sourceFileName, string destinationFileName)
        {
            _sourceFileName = sourceFileName;
            _destinationFileName = destinationFileName;
        }
        
        public void Sort()
        {
            var watch = new Stopwatch();
            watch.Start();
            
            Console.WriteLine($"Loading text file...");
            
            var lines = ReadFileLines();
            
            Console.WriteLine($"File loaded, took {watch.Elapsed}");
            watch.Restart();
            
            Console.WriteLine($"Sorting...");
            
            lines.Sort(new FileLineComparer());
            
            Console.WriteLine($"Sorted, took {watch.Elapsed}");
            watch.Restart();

            Console.WriteLine($"Saving result to file...");

            var streamWriter = new StreamWriter(_destinationFileName);
            foreach (var line in lines)
            {
                streamWriter.WriteLine(line.ToString());
            }
            
            Console.WriteLine($"Saved file, took {watch.Elapsed}");
            watch.Stop();
        }

        private List<FileLine> ReadFileLines()
        {
            const int bufferSize = 1048576;
            
            using var fileStream = File.OpenRead(_sourceFileName);
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize);
            
            ReadOnlySpan<char> line;
            var lines = new List<FileLine>();

            while ((line = streamReader.ReadLine().AsSpan()) != null)
            {
                var dotPosition = line.IndexOf(". ");

                lines.Add(new FileLine
                {
                    Number = int.Parse(line[..dotPosition]),
                    String = line[(dotPosition + 2)..].ToString()
                });
            }

            return lines;
        }
    }
}