using System;

namespace GiantTextFileSorter.Generator
{
    public class StringGenerator : IStringGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}