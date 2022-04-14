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
            var lines = await ReadFileLinesAsync();

            await using var streamWriter = new StreamWriter(_destinationFileName);
            
            lines.Sort(new FileLineComparer());
            
            // lines.Sort((l1, l2) => string.Compare(l1.String, l2.String, StringComparison.Ordinal));
            
            foreach (var line in lines)
            {
                await streamWriter.WriteLineAsync(line.ToString());
            }    
            
            // foreach (var line in lines.OrderBy(l => l.String).ThenBy(l => l.Number))
            // {
            //     await streamWriter.WriteLineAsync(line.ToString());
            // }            
        }

        private async Task<List<FileLine>> ReadFileLinesAsync()
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