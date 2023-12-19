namespace Godot.Extensions;

public static class RandomNumberGeneratorExtensions
{
    public static Color RandomColor(
        this RandomNumberGenerator randomNumberGenerator,
        float saturationMin,
        float saturationMax,
        float valueMin,
        float valueMax
    ) =>
        randomNumberGenerator.RandomColor(0, 1, saturationMin, saturationMax, valueMin, valueMax);

    public static Color RandomColor(
        this RandomNumberGenerator randomNumberGenerator,
        float hueMin,
        float hueMax,
        float saturationMin,
        float saturationMax,
        float valueMin,
        float valueMax
    )
    {
        return Color.FromHsv(
            randomNumberGenerator.RandfRange(hueMin, hueMax),
            randomNumberGenerator.RandfRange(saturationMin, saturationMax),
            randomNumberGenerator.RandfRange(valueMin, valueMax)
        );
    }
}