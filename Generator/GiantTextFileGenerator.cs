using System.IO;
using System.Text;
using GiantTextFileSorter.Generator.Builders;
using GiantTextFileSorter.Generator.Generators;
using GiantTextFileSorter.Generator.Repositories;

namespace GiantTextFileSorter.Generator
{
    public class GiantTextFileGenerator
    {
        private const int PercentOfDuplicates = 10;
        private readonly StreamWriter _streamWriter;
        private readonly FileLineBuilder _fileLineBuilder;
        private readonly long _fileSize;
        
        public GiantTextFileGenerator(string fileName, long fileSize)
        {
            _fileSize = fileSize;
            
            _streamWriter = new StreamWriter(fileName);
            _fileLineBuilder = new FileLineBuilder(new PositiveNumberGenerator(), new CachedStringGeneratorDecorator(new StringGenerator(new WordsRepository(new PositiveNumberGenerator())), PercentOfDuplicates));
        }

        public void Generate()
        {
            while (true)
            {
                var fileLine = _fileLineBuilder.Build();
                var fileLineLength = Encoding.Unicode.GetByteCount(fileLine.ToString());

                // Exit if file size exceeds the limit when new line is added
                // StreamWriter uses buffering (with a 4 kilobyte buffer size), so checking whether the limit was exceeded
                // won't be completely accurate, but for giant files it doesn't really matter
                if (_streamWriter.BaseStream.Length + fileLineLength > _fileSize)
                {
                    break;
                }
                
                _streamWriter.WriteLine(fileLine);
            }
        }
    }
}