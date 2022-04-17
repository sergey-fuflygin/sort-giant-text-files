using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GiantTextFileSorter.Common;
using GiantTextFileSorter.Sorter.Comparers;
using GiantTextFileSorter.Sorter.Extensions;

namespace GiantTextFileSorter.Sorter
{
    public class ExternalMergeSorter
    {
        private const string UnsortedFileExtension = ".unsorted.txt";
        private const string SortedFileExtension = ".sorted.txt";
        
        private const int FilesPerRun = 10; // how many files to process per run

        private const int BufferSize = 1048576;
        
        
        private long _maxUnsortedRows;
        
        private string[] _unsortedRows;
        
        private double _totalFilesToMerge;
        private int _mergeFilesProcessed;
        private readonly ExternalMergeSorterOptions _options;
        private const string TempFileExtension = ".tmp";

        public ExternalMergeSorter()
        {
            _totalFilesToMerge = 0;
            _mergeFilesProcessed = 0;
            _options = new ExternalMergeSorterOptions();
            _unsortedRows = Array.Empty<string>();
        }

        public async Task Sort(Stream source, Stream target)
        {
            var files = SplitFile(source);
            
            _unsortedRows = new string[_maxUnsortedRows];
            if (files.Count == 1)
            {
                SortFile2(File.OpenRead(files.First()), target);
                return;
            }
            var sortedFiles = await SortFiles(files);
            
            var done = false;
            const int size = 10; // files per run
            _totalFilesToMerge = sortedFiles.Count;
            var result = sortedFiles.Count / size;
            
            while (!done)
            {
                if (result <= 0)
                {
                    done = true;
                }
                _totalFilesToMerge += result;
                result /= size;
            }
            
            await MergeFiles(sortedFiles, target);
        }

        private IReadOnlyCollection<string> SplitFile(Stream sourceStream)
        {
            const int fileSize = 1024 * 1024 * 1024; // 1 GB
            const char newLineSeparator = '\n';
            var buffer = new byte[fileSize];
            var extraBuffer = new List<byte>();
            var filenames = new List<string>();
            
            using (sourceStream)
            {
                var currentFile = 0L;
                while (sourceStream.Position < sourceStream.Length)
                {
                    var totalRows = 0;
                    var runBytesRead = 0;
                    while (runBytesRead < fileSize)
                    {
                        var value = sourceStream.ReadByte();
                        if (value == -1)
                        {
                            break;
                        }

                        var @byte = (byte)value;
                        buffer[runBytesRead] = @byte;
                        runBytesRead++;
                        if (@byte == newLineSeparator)
                        {
                            totalRows++;
                        }
                    }

                    var extraByte = buffer[fileSize - 1];

                    while (extraByte != newLineSeparator)
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

                    if (totalRows > _maxUnsortedRows)
                    {
                        _maxUnsortedRows = totalRows;
                    }

                    filenames.Add(filename);
                    extraBuffer.Clear();
                }

                return filenames;
            }
        }

        private async Task<IReadOnlyList<string>> SortFiles(IReadOnlyCollection<string> unsortedFileNames)
        {
            var sortedFiles = new List<string>(unsortedFileNames.Count);
            
            foreach (var unsortedFileName in unsortedFileNames)
            {
                var sortedFilename = unsortedFileName.Replace(UnsortedFileExtension, SortedFileExtension);
                
                SortFile2(File.OpenRead(unsortedFileName), File.OpenWrite(sortedFilename));
                
                File.Delete(unsortedFileName);
                
                sortedFiles.Add(sortedFilename);
            }
            
            return sortedFiles;
        }

        // private async Task SortFile(Stream unsortedFileStream, Stream sortedFileStream)
        // {
        //     const int bufferSize = 1048576; // 65536
        //
        //     using var streamReader = new StreamReader(unsortedFileStream, bufferSize: bufferSize);
        //     var counter = 0;
        //     
        //     while (!streamReader.EndOfStream)
        //     {
        //         _unsortedRows[counter++] = (await streamReader.ReadLineAsync())!;
        //     }
        //     
        //     Array.Sort(_unsortedRows, new TextLineComparer());
        //     
        //     await using var streamWriter = new StreamWriter(sortedFileStream, bufferSize: bufferSize);
        //     
        //     // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //     foreach (var row in _unsortedRows.Where(x => x != null))
        //     {
        //         await streamWriter.WriteLineAsync(row);
        //     }
        //     
        //     Array.Clear(_unsortedRows, 0, _unsortedRows.Length);
        // }
        
        private void SortFile2(Stream unsortedFileStream, Stream sortedFileStream)
        {
            // 1. Read
            const int bufferSize = 1048576; // 65536
            
            using var streamReader = new StreamReader(unsortedFileStream, bufferSize: bufferSize);
            
            ReadOnlySpan<char> readOnlySpan;
            var lines = new List<FileLine>();

            while ((readOnlySpan = streamReader.ReadLine().AsSpan()) != null)
            {
                var dotPosition = readOnlySpan.IndexOf(". ");

                lines.Add(new FileLine
                {
                    Number = int.Parse(readOnlySpan[..dotPosition]),
                    String = readOnlySpan[(dotPosition + 2)..].ToString()
                });
            }
            
            // 2. Sort
            lines.Sort(new FileLineComparer());

            // 3. Write
            var streamWriter = new StreamWriter(sortedFileStream, bufferSize: bufferSize);
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

                sortedFiles = Directory.GetFiles(ExternalMergeSorterOptions.FileLocation, $"*{SortedFileExtension}")
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
                    ExternalMergeSortMergeOptions.ProgressHandler?.Report(++_mergeFilesProcessed / _totalFilesToMerge);
                    continue;
                }

                var value = streamReaders[streamReaderIndex].ReadLine();
                var fileLine = GetFileLine(value);
                rows[0] = new Row(fileLine, streamReaderIndex);
            }

            CleanupRun(streamReaders, filesToMerge);
        }

        /// <summary>
        /// Creates a StreamReader for each sorted sourceStream.
        /// Reads one line per StreamReader to initialize the rows list.
        /// </summary>
        private (StreamReader[] StreamReaders, List<Row> rows) InitializeStreamReaders(IReadOnlyList<string> sortedFiles)
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

        private FileLine GetFileLine(ReadOnlySpan<char> span)
        {
            var dotPosition = span.IndexOf(". ");

            return new FileLine
            {
                Number = int.Parse(span[..dotPosition]),
                String = span[(dotPosition + 2)..].ToString()
            };
        }

        /// <summary>
        /// Disposes all StreamReaders
        /// Renames old files to a temporary name and then deletes them.
        /// Reason for renaming first is that large files can take quite some time to remove
        /// and the .Delete call returns immediately.
        /// </summary>
        /// <param name="streamReaders"></param>
        /// <param name="filesToMerge"></param>
        private void CleanupRun(IReadOnlyList<StreamReader> streamReaders, IReadOnlyList<string> filesToMerge)
        {
            for (var i = 0; i < streamReaders.Count; i++)
            {
                streamReaders[i].Dispose();
                // RENAME BEFORE DELETION SINCE DELETION OF LARGE FILES CAN TAKE SOME TIME
                // WE DONT WANT TO CLASH WHEN WRITING NEW FILES.
                var temporaryFilename = $"{filesToMerge[i]}.removal.txt";
                File.Move(filesToMerge[i], temporaryFilename);
                File.Delete(temporaryFilename);
            }
        }
    }
}