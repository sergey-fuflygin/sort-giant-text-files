using CommandLine;

namespace GiantTextFileSorter.Sorter.Models
{
    public class CommandLineOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source file to be sorted.")]
        public string SourceFile { get; set; }
        
        [Option('t', "target", Required = true, HelpText = "Target file.")]
        public string TargetFile { get; set; }
    }
}