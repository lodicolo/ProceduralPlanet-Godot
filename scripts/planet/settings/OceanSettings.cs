using System;
using Godot;
using ProceduralPlanet.Utilities;
using static ProceduralPlanet.Utilities.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.settings;

[Tool]
public partial class OceanSettings : SettingsResource
{
    private readonly RandomNumberGenerator rng = new();

    private float _alphaMultiplier = 10;
    private Color _colorA;
    private Color _colorB;
    private float _depthMultiplier = 70;
    private Color _foamColor;
    private float _foamEdgeFalloffBias = 0.5f;
    private float _foamFalloffDistance = 0.5f;
    private float _foamLeadingEdgeFalloff = 1;
    private float _foamNoiseScale = 1;
    private Texture2D? _foamNoiseTexture;
    private float _level;
    private float _refractionScale = 1;
    private float _shoreWaveHeight = 0.1f;
    private float _smoothness = 0.92f;
    private Color _specularColor = new Color(1f, 1f, 1f);
    private float _specularIntensity = 0.5f;
    private Texture2D? _waveNormalA;
    private Texture2D? _waveNormalB;
    private float _waveScale = 15;
    private float _waveSpeed = 0.5f;
    private float _waveStrength = 0.15f;

    [Export]
    public float AlphaMultiplier
    {
        get => _alphaMultiplier;
        set
        {
            if (SetIfChanged(ref _alphaMultiplier, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color ColorA
    {
        get => _colorA;
        set
        {
            if (SetIfChanged(ref _colorA, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color ColorB
    {
        get => _colorB;
        set
        {
            if (SetIfChanged(ref _colorB, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float DepthMultiplier
    {
        get => _depthMultiplier;
        set
        {
            if (SetIfChanged(ref _depthMultiplier, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color FoamColor
    {
        get => _foamColor;
        set
        {
            if (SetIfChanged(ref _foamColor, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float FoamEdgeFalloffBias
    {
        get => _foamEdgeFalloffBias;
        set
        {
            if (SetIfChanged(ref _foamEdgeFalloffBias, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float FoamFalloffDistance
    {
        get => _foamFalloffDistance;
        set
        {
            if (SetIfChanged(ref _foamFalloffDistance, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float FoamLeadingEdgeFalloff
    {
        get => _foamLeadingEdgeFalloff;
        set
        {
            if (SetIfChanged(ref _foamLeadingEdgeFalloff, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float FoamNoiseScale
    {
        get => _foamNoiseScale;
        set
        {
            if (SetIfChanged(ref _foamNoiseScale, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Texture2D? FoamNoiseTexture
    {
        get => _foamNoiseTexture;
        set
        {
            if (SetIfChanged(ref _foamNoiseTexture, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float RefractionScale
    {
        get => _refractionScale;
        set
        {
            if (SetIfChanged(ref _refractionScale, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float ShoreWaveHeight
    {
        get => _shoreWaveHeight;
        set
        {
            if (SetIfChanged(ref _shoreWaveHeight, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float Smoothness
    {
        get => _smoothness;
        set
        {
            if (SetIfChanged(ref _smoothness, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Color SpecularColor
    {
        get => _specularColor;
        set
        {
            if (SetIfChanged(ref _specularColor, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float SpecularIntensity
    {
        get => _specularIntensity;
        set
        {
            if (SetIfChanged(ref _specularIntensity, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Texture2D? WaveNormalA
    {
        get => _waveNormalA;
        set
        {
            if (SetIfChanged(ref _waveNormalA, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public Texture2D? WaveNormalB
    {
        get => _waveNormalB;
        set
        {
            if (SetIfChanged(ref _waveNormalB, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float WaveScale
    {
        get => _waveScale;
        set
        {
            if (SetIfChanged(ref _waveScale, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float WaveSpeed
    {
        get => _waveSpeed;
        set
        {
            if (SetIfChanged(ref _waveSpeed, value))
            {
                EmitChanged();
            }
        }
    }

    [Export]
    public float WaveStrength
    {
        get => _waveStrength;
        set
        {
            if (SetIfChanged(ref _waveStrength, value))
            {
                EmitChanged();
            }
        }
    }


    [Export(PropertyHint.Range, "0,1")] public float Level
    {
        get => _level;
        set
        {
            if (SetIfChanged(ref _level, value))
            {
                EmitChanged();
            }
        }
    }

    public void SetProperties(ShaderMaterial? material, int seed, bool randomize)
    {
        if (material == default)
        {
            return;
        }

        material.SetShaderParameter("DepthMultiplier", DepthMultiplier);
        material.SetShaderParameter("AlphaMultiplier", AlphaMultiplier);

        material.SetShaderParameter("WaveNormalA", WaveNormalA ?? default(Variant));
        material.SetShaderParameter("WaveNormalB", WaveNormalB ?? default(Variant));
        material.SetShaderParameter("WaveStrength", WaveStrength);
        material.SetShaderParameter("WaveNormalScale", WaveScale);
        material.SetShaderParameter("WaveSpeed", WaveSpeed);
        material.SetShaderParameter("ShoreWaveHeight", ShoreWaveHeight);
        material.SetShaderParameter("Smoothness", Smoothness);
        material.SetShaderParameter("SpecularIntensity", SpecularIntensity);
        material.SetShaderParameter("FoamNoiseTexture", FoamNoiseTexture ?? default(Variant));
        material.SetShaderParameter("FoamColor", FoamColor);
        material.SetShaderParameter("FoamNoiseScale", FoamNoiseScale);
        material.SetShaderParameter("FoamFalloffDistance", FoamFalloffDistance);
        material.SetShaderParameter("FoamLeadingEdgeFalloff", FoamLeadingEdgeFalloff);
        material.SetShaderParameter("FoamEdgeFalloffBias", FoamEdgeFalloffBias);
        material.SetShaderParameter("RefractionScale", RefractionScale);

        var colorA = ColorA;
        var colorB = ColorB;
        var specularColor = SpecularColor;
        if (randomize)
        {
            rng.Seed = (ulong)seed;
            rng.Randomize();
            colorA = Color.FromHsv(rng.Randf(), rng.RandfRange(0.6f, 0.8f), rng.RandfRange(0.65f, 1.0f));
            colorB = ColorA.TweakHsv(
                Math.Sign(rng.Randf()) * 0.2f,
                Math.Sign(rng.Randf()) * 0.2f,
                rng.RandfRange(-0.5f, 0.4f)
            );
            specularColor = new Color(1f, 1f, 1f);
        }

        material.SetShaderParameter("ColA", colorA);
        material.SetShaderParameter("ColB", colorB);
        material.SetShaderParameter("SpecularCol", specularColor);
    }
}