using System.Collections.Generic;

namespace GiantTextFileSorter.Generator.Generators
{
    /// <summary>
    /// Add duplicates functionality to StringGenerator class
    /// </summary>
    public class CachedStringGeneratorDecorator : IStringGenerator
    {
        private readonly IStringGenerator _innerGenerator;
        
        private readonly Queue<string> _duplicates;
        private readonly int _percentOfDuplicates;
        private int _stringsCount;
        private const int CacheSize = 1000;
        
        public CachedStringGeneratorDecorator(IStringGenerator innerGenerator, int percentOfDuplicates)
        {
            _innerGenerator = innerGenerator;
            _percentOfDuplicates = percentOfDuplicates;
            _duplicates = new Queue<string>(CacheSize);
        }
        
        public string Generate()
        {
            var @string = _innerGenerator.Generate();
            _stringsCount++;

            if (_stringsCount % (100 / _percentOfDuplicates) == 0)
            {
                return _duplicates.Dequeue() ?? @string;
            }

            if (_duplicates.Count < CacheSize)
            {
                _duplicates.Enqueue(@string);
            }

            return @string;
        }
    }
}