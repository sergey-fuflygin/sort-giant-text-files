namespace GiantTextFileSorter.Generator
{
    internal static class Program
    {
        private static void Main(string[] args) 
        {
            var t = new GiantTextFileGenerator("random.txt", 1024 * 1024 * 5); // 5 MB
            t.Generate();
        }
    }
}