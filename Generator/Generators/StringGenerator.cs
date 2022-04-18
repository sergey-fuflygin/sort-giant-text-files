using System.Text;
using GiantTextFileSorter.Generator.Extensions;
using GiantTextFileSorter.Generator.Repositories;

namespace GiantTextFileSorter.Generator.Generators
{
    /// <summary>
    /// Generates random string from English words dictionary.
    /// String contains at lest one word.
    /// First letter of first word is in upper case.
    /// Words are separated with a space.
    /// Words can be repeated.
    /// String contains 5 words maximum.
    /// Examples are:
    ///     Apple
    ///     Something something something
    ///     Cherry is the best
    /// </summary>
    public class StringGenerator : IStringGenerator
    {
        private readonly IWordsRepository _wordsRepository;
        private readonly IPositiveNumberGenerator _positiveNumberGenerator;
        private const int MaxWordsCount = 5;

        public StringGenerator(IWordsRepository wordsRepository)
        {
            _wordsRepository = wordsRepository;
            _positiveNumberGenerator = new PositiveNumberGenerator(MaxWordsCount);
        }
        
        public string Generate()
        {
            var sb = new StringBuilder();
            
            // String must contain at least one word. And first word begins with capital letter
            sb.Append(_wordsRepository.GetRandomWord().FirstCharToUpper());
            
            for (var i = 2; i <= _positiveNumberGenerator.Generate(); i++)
            {
                sb.Append($" {_wordsRepository.GetRandomWord()}");
            }

            return sb.ToString();
        }
    }
}