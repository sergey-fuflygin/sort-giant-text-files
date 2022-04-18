using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GiantTextFileSorter.Common;
using GiantTextFileSorter.Sorter.Comparers;
using GiantTextFileSorter.Sorter.Extensions;
using GiantTextFileSorter.Sorter.Models;

namespace GiantTextFileSorter.Sorter
{
    // Inspired by https://josef.codes/sorting-really-large-files-with-c-sharp/
    public class GiantTextFileSorter
    {
        private const int BufferSize = 1048576;
        private const int SplitFileSize = 1024 * 1024 * 1024; // 1 GB
        private const int FilesPerRun = 10;
        private const string UnsortedFileExtension = ".unsorted.txt";
        private const string SortedFileExtension = ".sorted.txt";
        private const string TempFileExtension = ".tmp.txt";
        private const char NewLineSeparator = '\n';
        private int _maxRowsCount;

        public async Task Sort(Stream source, Stream target)
        {
            // 1. Split
            var watch = new Stopwatch();
            watch.Start();
            
            Console.WriteLine($"Splitting text file...");
            
            var files = SplitFile(source);
            
            Console.WriteLine($"Split to {files.Count} file(s) in {watch.Elapsed:m\\:ss}.");
            watch.Restart();

            if (files.Count == 1)
            {
                Console.WriteLine($"Sorting file...");

                SortFile(File.OpenRead(files.First()), target);
                File.Delete(files.First());

                Console.WriteLine($"Sorted in {watch.Elapsed:m\\:ss}.");

                return;
            }
            
            // 2. Sort
            Console.WriteLine($"Sorting files...");
            
            var sortedFiles = SortFiles(files);
            
            Console.WriteLine($"Sorted in {watch.Elapsed:m\\:ss}");
            watch.Restart();
            
            // 3. Merge
            Console.WriteLine($"Merging sorted files into single one...");
            
            var done = false;
            var result = sortedFiles.Count / FilesPerRun;
            
            while (!done)
            {
                if (result <= 0)
                {
                    done = true;
                }

                result /= FilesPerRun;
            }
            
            await MergeFiles(sortedFiles, target);
            
            Console.WriteLine($"Merged files in {watch.Elapsed:m\\:ss}.");
        }

        private IReadOnlyCollection<string> SplitFile(Stream sourceStream)
        {
            var buffer = new byte[SplitFileSize];
            var extraBuffer = new List<byte>();
            var filenames = new List<string>();
            
            using (sourceStream)
            {
                var currentFile = 0L;
                while (sourceStream.Position < sourceStream.Length)
                {
                    var totalRows = 0;
                    var runBytesRead = 0;
                    while (runBytesRead < SplitFileSize)
                    {
                        var value = sourceStream.ReadByte();
                        if (value == -1)
                        {
                            break;
                        }

                        var @byte = (byte)value;
                        buffer[runBytesRead] = @byte;
                        runBytesRead++;
                        if (@byte == NewLineSeparator)
                        {
                            totalRows++;
                        }
                    }

                    var extraByte = buffer[SplitFileSize - 1];

                    while (extraByte != NewLineSeparator)
                    {
                        var flag = sourceStream.ReadByte();
                        if (flag == -1)
                        {
                            break;
                        }
                        extraByte = (byte)flag;
                        extraBuffer.Add(extraByte);
                    }

                    var filename = $"{++currentFile}.unsorted.txt";
                    using var unsortedFile = File.Create(filename);
                    unsortedFile.Write(buffer, 0, runBytesRead);
                    if (extraBuffer.Count > 0)
                    {
                        totalRows++;
                        unsortedFile.Write(extraBuffer.ToArray(), 0, extraBuffer.Count);
                    }
                    
                    if (totalRows > _maxRowsCount)
                    {
                        _maxRowsCount = totalRows;
                    }

                    filenames.Add(filename);
                    extraBuffer.Clear();
                }

                return filenames;
            }
        }

        private IReadOnlyList<string> SortFiles(IReadOnlyCollection<string> unsortedFileNames)
        {
            var sortedFiles = new List<string>(unsortedFileNames.Count);
            
            foreach (var unsortedFileName in unsortedFileNames)
            {
                var sortedFilename = unsortedFileName.Replace(UnsortedFileExtension, SortedFileExtension);
                
                SortFile(File.OpenRead(unsortedFileName), File.OpenWrite(sortedFilename));
                
                File.Delete(unsortedFileName);
                
                sortedFiles.Add(sortedFilename);
            }
            
            return sortedFiles;
        }

        private void SortFile(Stream unsortedFileStream, Stream sortedFileStream)
        {
            // 1. Read
            using var streamReader = new StreamReader(unsortedFileStream, bufferSize: BufferSize);
            
            ReadOnlySpan<char> span;
            var lines = new List<FileLine>(_maxRowsCount);

            while ((span = streamReader.ReadLine().AsSpan()) != null)
            {
                lines.Add(GetFileLine(span));
            }
            
            // 2. Sort
            lines.Sort(new FileLineComparer());

            // 3. Write
            var streamWriter = new StreamWriter(sortedFileStream, bufferSize: BufferSize);
            foreach (var line in lines)
            {
                streamWriter.WriteLine(line.ToString());
            }
            streamWriter.Close();
        }
        
        private async Task MergeFiles(IReadOnlyList<string> sortedFiles, Stream target)
        {
            var done = false;
            while (!done)
            {
                var finalRun = sortedFiles.Count <= FilesPerRun;

                if (finalRun)
                {
                    await Merge(sortedFiles, target);
                    return;
                }

                var runs = sortedFiles.ChunkBy(FilesPerRun);
                var chunkCounter = 0;
                foreach (var files in runs)
                {
                    var outputFilename = $"{++chunkCounter}{SortedFileExtension}{TempFileExtension}";
                    if (files.Count == 1)
                    {
                        File.Move(files.First(), outputFilename.Replace(TempFileExtension, string.Empty));
                        continue;
                    }

                    var outputStream = File.OpenWrite(outputFilename);
                    await Merge(files, outputStream);
                    File.Move(outputFilename, outputFilename.Replace(TempFileExtension, string.Empty), true);
                }
                
                sortedFiles = Directory.GetFiles(AppContext.BaseDirectory, $"*{SortedFileExtension}")
                    .OrderBy(x =>
                    {
                        var filename = Path.GetFileNameWithoutExtension(x);
                        return int.Parse(filename);
                    })
                    .ToArray();

                if (sortedFiles.Count > 1)
                {
                    continue;
                }

                done = true;
            }
        }

        private async Task Merge(IReadOnlyList<string> filesToMerge, Stream outputStream)
        {
            var (streamReaders, rows) = InitializeStreamReaders(filesToMerge);
            var finishedStreamReaders = new List<int>(streamReaders.Length);
            var done = false;
            await using var outputWriter = new StreamWriter(outputStream, bufferSize: BufferSize);

            while (!done)
            {
                rows.Sort((row1, row2) => new FileLineComparer().Compare(row1.FileLine, row2.FileLine));
                var valueToWrite = rows[0].FileLine;
                var streamReaderIndex = rows[0].StreamReader;
                await outputWriter.WriteLineAsync(valueToWrite.ToString());

                if (streamReaders[streamReaderIndex].EndOfStream)
                {
                    var indexToRemove = rows.FindIndex(x => x.StreamReader == streamReaderIndex);
                    rows.RemoveAt(indexToRemove);
                    finishedStreamReaders.Add(streamReaderIndex);
                    done = finishedStreamReaders.Count == streamReaders.Length;
                    continue;
                }

                var value = streamReaders[streamReaderIndex].ReadLine();
                var fileLine = GetFileLine(value);
                rows[0] = new Row(fileLine, streamReaderIndex);
            }

            CleanupRun(streamReaders, filesToMerge);
        }

        private static (StreamReader[] StreamReaders, List<Row> rows) InitializeStreamReaders(IReadOnlyList<string> sortedFiles)
        {
            var streamReaders = new StreamReader[sortedFiles.Count];
            var rows = new List<Row>(sortedFiles.Count);
            for (var i = 0; i < sortedFiles.Count; i++)
            {
                var sortedFileStream = File.OpenRead(sortedFiles[i]);
                streamReaders[i] = new StreamReader(sortedFileStream, bufferSize: BufferSize);
                var span = streamReaders[i].ReadLine().AsSpan();
                var fileLine = GetFileLine(span);
                var row = new Row(fileLine, i);
                rows.Add(row);
            }

            return (streamReaders, rows);
        }

        private static FileLine GetFileLine(ReadOnlySpan<char> span)
        {
            var dotPosition = span.IndexOf(". ");

            return new FileLine
            {
                Number = int.Parse(span[..dotPosition]),
                String = span[(dotPosition + 2)..].ToString()
            };
        }

        private static void CleanupRun(IReadOnlyList<StreamReader> streamReaders, IReadOnlyList<string> filesToMerge)
        {
            for (var i = 0; i < streamReaders.Count; i++)
            {
                streamReaders[i].Dispose();
                var temporaryFilename = $"{filesToMerge[i]}.removal.txt";
                File.Move(filesToMerge[i], temporaryFilename);
                File.Delete(temporaryFilename);
            }
        }
    }
}