namespace GiantTextFileSorter.Generator.NumberGenerator
{
    public interface IPositiveNumberGenerator
    {
        int Generate();
        int Generate(int minValue, int maxValue);
    }
}