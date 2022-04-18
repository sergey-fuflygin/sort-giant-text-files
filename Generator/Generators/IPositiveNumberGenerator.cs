namespace GiantTextFileSorter.Generator.Generators
{
    public interface IPositiveNumberGenerator
    {
        int Generate();
        int Generate(int minValue, int maxValue);
    }
}