using System.Collections.Generic;
using System.IO;
using GiantTextFileSorter.Generator.Generators;

namespace GiantTextFileSorter.Generator.Repositories
{
    public class WordsRepository : IWordsRepository
    {
        private const string FileName = "english_words.txt";
        private readonly IPositiveNumberGenerator _positiveNumberGenerator;
        private readonly List<string> _words;
        
        public WordsRepository(IPositiveNumberGenerator positiveNumberGenerator)
        {
            _positiveNumberGenerator = positiveNumberGenerator;
            _words = new List<string>(File.ReadAllLines(FileName));
        }

        public string GetRandomWord() =>
            _words[_positiveNumberGenerator.Generate(0, _words.Count - 1)];
    }
}