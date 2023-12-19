using System;

namespace Godot.Extensions;

public static class ColorExtensions
{
    public static Color TweakHsv(this Color color, float deltaH, float deltaS, float deltaV) =>
        Color.FromHsv(
            MathF.IEEERemainder(color.H + deltaH, 1),
            Math.Clamp(color.S + deltaS, 0, 1),
            Math.Clamp(color.V + deltaV, 0, 1)
        );
}