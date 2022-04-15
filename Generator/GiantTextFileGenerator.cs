using System.IO;
using System.Text;
using GiantTextFileSorter.Generator.NumberGenerator;
using GiantTextFileSorter.Generator.Repositories;
using GiantTextFileSorter.Generator.StringGenerator;

namespace GiantTextFileSorter.Generator
{
    public class GiantTextFileGenerator
    {
        private readonly StreamWriter _streamWriter;

        private readonly FileLineBuilder _fileLineBuilder;

        private readonly int _fileSize;
        
        public GiantTextFileGenerator(string fileName, int fileSize)
        {
            _fileSize = fileSize;
            
            _streamWriter = new StreamWriter(fileName);
            _fileLineBuilder = new FileLineBuilder(new PositiveNumberGenerator(fileSize), new CachedStringGeneratorDecorator(new StringGenerator.StringGenerator(new WordsRepository(new PositiveNumberGenerator())), 10));
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