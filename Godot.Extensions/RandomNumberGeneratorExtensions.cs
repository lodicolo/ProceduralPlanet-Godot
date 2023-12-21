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

    public static Vector2 RandomUnitCircleVector(this RandomNumberGenerator randomNumberGenerator)
    {
        Vector2 vector = new(randomNumberGenerator.RandfRange(-1, 1), randomNumberGenerator.RandfRange(-1, 1));
        if (vector.LengthSquared() < 0.001)
        {
            vector = Vector2.Right;
        }

        return vector.Normalized();
    }

    public static Vector3 RandomUnitSphereVector(this RandomNumberGenerator randomNumberGenerator)
    {

        Vector3 vector = new(
            randomNumberGenerator.RandfRange(-1, 1),
            randomNumberGenerator.RandfRange(-1, 1),
            randomNumberGenerator.RandfRange(-1, 1)
        );
        if (vector.LengthSquared() < 0.001)
        {
            vector = Vector3.Right;
        }

        return vector.Normalized();
    }
}