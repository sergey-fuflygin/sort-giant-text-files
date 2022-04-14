using System;

namespace GiantTextFileSorter.Generator
{
    public class PositiveNumberGenerator : IPositiveNumberGenerator
    {
        private static Random Random => new Random();
        private readonly int _maxValue;

        public PositiveNumberGenerator(int maxValue = int.MaxValue)
        {
            _maxValue = maxValue;
        }

        public int Generate()
        {
            return Random.Next(1, _maxValue);
        }
    }
}