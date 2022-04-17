using System;

namespace GiantTextFileSorter.Generator.NumberGenerator
{
    public class PositiveNumberGenerator : IPositiveNumberGenerator
    {
        private static readonly Random Random = new Random();
        private const int MinValue = 1;
        private readonly int _maxValue;

        public PositiveNumberGenerator(int maxValue = int.MaxValue - 1)
        {
            _maxValue = maxValue;
        }

        public int Generate() => Generate(MinValue, _maxValue);

        public int Generate(int minValue, int maxValue) => Random.Next(MinValue, maxValue + 1);
    }
}