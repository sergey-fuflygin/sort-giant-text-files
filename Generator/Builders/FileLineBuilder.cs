using GiantTextFileSorter.Common.Models;
using GiantTextFileSorter.Generator.Generators;

namespace GiantTextFileSorter.Generator.Builders
{
    public class FileLineBuilder
    {
        private readonly IPositiveNumberGenerator _positiveNumberGenerator;
        private readonly IStringGenerator _stringGenerator;

        public FileLineBuilder(IPositiveNumberGenerator positiveNumberGenerator, IStringGenerator stringGenerator)
        {
            _positiveNumberGenerator = positiveNumberGenerator;
            _stringGenerator = stringGenerator;
        }

        private FileLine _fileLine;

        public FileLine Build()
        {
            return SetNumber(_positiveNumberGenerator.Generate())
                .SetString(_stringGenerator.Generate())
                .Create();
        }
        
        private FileLine Create()
        {
            return _fileLine;
        }
        
        private FileLineBuilder SetNumber(int number)
        {
            _fileLine.Number = number;
            return this;
        }
        
        private FileLineBuilder SetString(string @string)
        {
            _fileLine.String = @string;
            return this;
        }
    }
}