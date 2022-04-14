using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        
        public async Task SortAsync()
        {
            var lines = await ReadFileLines();

            await using var streamWriter = new StreamWriter(_destinationFileName);
            foreach (var line in lines.OrderBy(l => l, new FileLineComparer()))
            {
                await streamWriter.WriteLineAsync(line.ToString());
            }            
        }

        private async Task<List<FileLine>> ReadFileLines()
        {
            using var streamReader = new StreamReader(_sourceFileName);
            var lines = new List<FileLine>();
            
            while (!streamReader.EndOfStream)
            {
                var strLine = await streamReader.ReadLineAsync();
                if (strLine == null) continue;

                var lineParts = strLine.Split(". ");
                var number = int.Parse(lineParts[0]);
                var @string = lineParts[1];

                var fileLine = new FileLine()
                {
                    Number = number,
                    String = @string
                };

                lines.Add(fileLine);
            }

            return lines;
        }
    }
}