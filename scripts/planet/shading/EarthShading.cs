using System;
using System.Diagnostics;
using Godot;
using ProceduralPlanet.scripts.planet.shading.colors;
using Godot.Extensions;

namespace ProceduralPlanet.scripts.planet.shading;

[Tool]
public partial class EarthShading : CelestialBodyShading
{
    private EarthColors? _customizedColors;
    private EarthColors? _randomizedColors;

    [Export] public EarthColors? CustomizedColors
    {
        get => _customizedColors;
        set
        {
            if (!PropertyHelper.SetIfChanged(ref _customizedColors, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            if (_customizedColors is not { } customizedColors)
            {
                return;
            }

            customizedColors.ResourceName = "Customized Colors";
            EmitChanged();
            customizedColors.Changed += EmitChanged;
        }
    }

    [Export] public EarthColors? RandomizedColors
    {
        get => _randomizedColors;
        set
        {
            if (!PropertyHelper.SetIfChanged(ref _randomizedColors, value, out var lastValue))
            {
                return;
            }

            if (lastValue is not null)
            {
                lastValue.Changed -= EmitChanged;
            }

            if (_randomizedColors is not { } randomizedColors)
            {
                return;
            }

            randomizedColors.ResourceName = "Randomized Colors";
            EmitChanged();
            randomizedColors.Changed += EmitChanged;
        }
    }

    public override void SetTerrainProperties(ShaderMaterial? material, Vector2 heightMinMax, float bodyScale)
    {
        base.SetTerrainProperties(material, heightMinMax, bodyScale);

        if (material is null)
        {
            return;
        }

        material.SetShaderParameter("HeightMinMax", heightMinMax);
        material.SetShaderParameter("OceanLevel", OceanSettings?.Level ?? default);
        material.SetShaderParameter("BodyScale", bodyScale);

        if (Randomize)
        {
            _randomizedColors = GetRandomColors(_rng);
            ApplyColors(material, _randomizedColors);
        }
        else
        {
            ApplyColors(material, CustomizedColors);
        }
    }

    private static void ApplyColors(ShaderMaterial material, EarthColors? colors)
    {
        if (colors == default)
        {
            Debug.WriteLine("Tried to apply null colors");
            return;
        }

        material.SetShaderParameter("FlatHighA", colors.FlatHighA);
        material.SetShaderParameter("FlatLowA", colors.FlatLowA);

        material.SetShaderParameter("FlatHighB", colors.FlatHighB);
        material.SetShaderParameter("FlatLowB", colors.FlatLowB);

        material.SetShaderParameter("ShoreHigh", colors.ShoreHigh);
        material.SetShaderParameter("ShoreLow", colors.ShoreLow);

        material.SetShaderParameter("SteepHigh", colors.SteepHigh);
        material.SetShaderParameter("SteepLow", colors.SteepLow);
    }

    private static EarthColors GetRandomColors(RandomNumberGenerator rng)
    {
        rng.Randomize();

        var flatLowA = rng.RandomColor(0.45f, 0.6f, 0.7f, 0.8f);
        var flatLowB = rng.RandomColor(0.45f, 0.6f, 0.7f, 0.8f);
        var shoreLow = rng.RandomColor(0.2f, 0.3f, 0.9f, 1.0f);
        var steepLow = rng.RandomColor(0.3f, 0.7f, 0.4f, 0.6f);
        var colors = new EarthColors
        {
            FlatLowA = flatLowA,
            FlatLowB = flatLowB,
            ShoreLow = shoreLow,
            SteepLow = steepLow,

            FlatHighA = flatLowA.TweakHsv(
                Math.Sign(rng.Randf()) * 0.2f,
                Math.Sign(rng.Randf()) * 0.15f,
                rng.RandfRange(-0.25f, 0.2f)
            ),
            FlatHighB = flatLowB.TweakHsv(
                Math.Sign(rng.Randf()) * 0.2f,
                Math.Sign(rng.Randf()) * 0.15f,
                rng.RandfRange(-0.25f, 0.2f)
            ),
            ShoreHigh = shoreLow.TweakHsv(
                Math.Sign(rng.Randf()) * 0.2f,
                Math.Sign(rng.Randf()) * 0.2f,
                rng.RandfRange(-0.3f, 0.2f)
            ),
            SteepHigh = steepLow.TweakHsv(
                Math.Sign(rng.Randf()) * 0.2f,
                Math.Sign(rng.Randf()) * 0.2f,
                rng.RandfRange(-0.35f, 0.2f)
            )
        };

        return colors;
    }
}