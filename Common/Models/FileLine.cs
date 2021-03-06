namespace GiantTextFileSorter.Common.Models
{
    public struct FileLine
    {
        public int Number { get;  set; }
        public string String { get; set; }
        
        public override string ToString() => $"{Number.ToString()}. {String}";
    }
}