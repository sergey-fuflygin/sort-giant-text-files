using CommandLine;

namespace GiantTextFileSorter.Generator.Models
{
    public class CommandLineOptions
    {
        [Option('f', "file", Required = true, HelpText = "File name.")]
        public string FileName { get; set; }
        
        [Option('s', "size", Required = true, HelpText = "File size in bytes.")]
        public long FileSize { get; set; }
    }
}